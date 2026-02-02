using System.Collections;
using UnityEngine;

public class HollowKnightMovement : MonoBehaviour
{
    public HollowKnightData Data;

    #region COMPONENTS
    public Rigidbody2D RB { get; private set; }
    public Animator AnimHandler { get; private set; }
    private SpriteRenderer spriteRenderer;
    #endregion

    #region STATE PARAMETERS
    public bool IsFacingRight { get; private set; }
    public bool IsJumping { get; private set; }
    public bool IsWallSliding { get; private set; }
    public bool IsDashing { get; private set; }
    public bool IsAttacking { get; private set; }

    // Timers
    public float LastOnGroundTime { get; private set; }
    public float LastOnWallTime { get; private set; }
    public float LastOnWallRightTime { get; private set; }
    public float LastOnWallLeftTime { get; private set; }

    // Jump
    private bool _isJumpCut;
    private bool _isJumpFalling;
    private int _airJumpsLeft;

    // Wall
    private int _lastWallJumpDir;
    private float _wallJumpStartTime;

    // Dash
    private int _dashesLeft;
    private bool _dashRefilling;
    private Vector2 _lastDashDir;
    private bool _isDashAttacking;
    #endregion

    #region INPUT PARAMETERS
    private Vector2 _moveInput;
    public float LastPressedJumpTime { get; private set; }
    public float LastPressedDashTime { get; private set; }
    public float LastPressedAttackTime { get; private set; }
    #endregion

    #region CHECK PARAMETERS
    [Header("Checks")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);
    [Space(5)]
    [SerializeField] private Transform _frontWallCheckPoint;
    [SerializeField] private Transform _backWallCheckPoint;
    [SerializeField] private Vector2 _wallCheckSize = new Vector2(0.5f, 1f);
    #endregion

    #region LAYERS & TAGS
    [Header("Layers & Tags")]
    [SerializeField] private LayerMask _groundLayer;
    #endregion

    #region PARTICLE EFFECTS (Optional)
    [Header("Effects")]
    [SerializeField] private ParticleSystem _dashEffect;
    [SerializeField] private ParticleSystem _jumpEffect;
    [SerializeField] private ParticleSystem _landEffect;
    [SerializeField] private TrailRenderer _dashTrail;
    #endregion

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        AnimHandler = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        SetGravityScale(Data.gravityScale);
        IsFacingRight = true;
        _airJumpsLeft = Data.airJumpsAmount;
        _dashesLeft = Data.dashAmount;
    }

    private void Update()
    {
        #region TIMERS
        LastOnGroundTime -= Time.deltaTime;
        LastOnWallTime -= Time.deltaTime;
        LastOnWallRightTime -= Time.deltaTime;
        LastOnWallLeftTime -= Time.deltaTime;

        LastPressedJumpTime -= Time.deltaTime;
        LastPressedDashTime -= Time.deltaTime;
        LastPressedAttackTime -= Time.deltaTime;
        #endregion

        #region INPUT HANDLER
        _moveInput.x = Input.GetAxisRaw("Horizontal");
        _moveInput.y = Input.GetAxisRaw("Vertical");

        if (_moveInput.x != 0)
            CheckDirectionToFace(_moveInput.x > 0);

        // Jump Input (Space, C, J, W, Up Arrow)
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.C) || 
            Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.W) || 
            Input.GetKeyDown(KeyCode.UpArrow))
        {
            OnJumpInput();
        }

        if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.C) || 
            Input.GetKeyUp(KeyCode.J) || Input.GetKeyUp(KeyCode.W) || 
            Input.GetKeyUp(KeyCode.UpArrow))
        {
            OnJumpUpInput();
        }

        // Dash Input (LeftShift, X, K)
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.X) || 
            Input.GetKeyDown(KeyCode.K))
        {
            OnDashInput();
        }

        // Attack Input (Z, Mouse0) - Preparado para futuro
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButtonDown(0))
        {
            OnAttackInput();
        }
        #endregion

        #region COLLISION CHECKS
        if (!IsDashing && !IsJumping)
        {
            // Ground Check
            bool wasGrounded = LastOnGroundTime > 0;
            if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer))
            {
                LastOnGroundTime = Data.coyoteTime;
                
                // Reset air jumps and dash on landing
                if (!wasGrounded)
                {
                    _airJumpsLeft = Data.airJumpsAmount;
                    
                    // Play land effect
                    if (_landEffect != null)
                        _landEffect.Play();
                }
            }

            // Right Wall Check
            if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && IsFacingRight)
                    || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !IsFacingRight)))
                LastOnWallRightTime = Data.coyoteTime;

            // Left Wall Check
            if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !IsFacingRight)
                || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && IsFacingRight)))
                LastOnWallLeftTime = Data.coyoteTime;

            LastOnWallTime = Mathf.Max(LastOnWallLeftTime, LastOnWallRightTime);
        }
        #endregion

        #region JUMP CHECKS
        if (IsJumping && RB.linearVelocity.y < 0)
        {
            IsJumping = false;
            _isJumpFalling = true;
        }

        if (Time.time - _wallJumpStartTime > Data.wallJumpTime)
        {
            // Reset wall jump state after timer
        }

        if (LastOnGroundTime > 0 && !IsJumping)
        {
            _isJumpCut = false;
            _isJumpFalling = false;
        }

        if (!IsDashing)
        {
            // Normal Jump
            if (CanJump() && LastPressedJumpTime > 0)
            {
                IsJumping = true;
                _isJumpCut = false;
                _isJumpFalling = false;
                Jump();
            }
            // Air Jump (Double Jump with Monarch Wings)
            else if (CanAirJump() && LastPressedJumpTime > 0)
            {
                _airJumpsLeft--;
                IsJumping = true;
                _isJumpCut = false;
                _isJumpFalling = false;
                Jump();
            }
            // Wall Jump
            else if (CanWallJump() && LastPressedJumpTime > 0)
            {
                _wallJumpStartTime = Time.time;
                _lastWallJumpDir = (LastOnWallRightTime > 0) ? -1 : 1;
                WallJump(_lastWallJumpDir);
            }
        }
        #endregion

        #region DASH CHECKS
        if (CanDash() && LastPressedDashTime > 0)
        {
            // Determine dash direction (8-directional like Hollow Knight)
            if (_moveInput != Vector2.zero)
            {
                _lastDashDir = _moveInput.normalized;
            }
            else
            {
                // Dash forward if no input
                _lastDashDir = IsFacingRight ? Vector2.right : Vector2.left;
            }

            IsDashing = true;
            IsJumping = false;
            _isJumpCut = false;

            StartCoroutine(nameof(StartDash), _lastDashDir);
        }
        #endregion

        #region WALL SLIDE CHECKS
        if (CanWallSlide() && ((LastOnWallLeftTime > 0 && _moveInput.x < 0) || (LastOnWallRightTime > 0 && _moveInput.x > 0)))
        {
            IsWallSliding = true;
        }
        else
        {
            IsWallSliding = false;
        }
        #endregion

        #region GRAVITY
        if (!_isDashAttacking)
        {
            if (IsWallSliding)
            {
                // Slow fall on wall
                SetGravityScale(0);
            }
            else if (RB.linearVelocity.y < 0 && _moveInput.y < 0)
            {
                // Fast fall when holding down
                SetGravityScale(Data.gravityScale * Data.fastFallGravityMult);
                RB.linearVelocity = new Vector2(RB.linearVelocity.x, Mathf.Max(RB.linearVelocity.y, -Data.maxFastFallSpeed));
            }
            else if (_isJumpCut)
            {
                // Higher gravity when jump button released
                SetGravityScale(Data.gravityScale * Data.jumpCutGravityMult);
                RB.linearVelocity = new Vector2(RB.linearVelocity.x, Mathf.Max(RB.linearVelocity.y, -Data.maxFallSpeed));
            }
            else if ((IsJumping || _isJumpFalling) && Mathf.Abs(RB.linearVelocity.y) < Data.jumpHangTimeThreshold)
            {
                // Floaty feel at jump apex (very Hollow Knight)
                SetGravityScale(Data.gravityScale * Data.jumpHangGravityMult);
            }
            else if (RB.linearVelocity.y < 0)
            {
                // Normal falling
                SetGravityScale(Data.gravityScale * Data.fallGravityMult);
                RB.linearVelocity = new Vector2(RB.linearVelocity.x, Mathf.Max(RB.linearVelocity.y, -Data.maxFallSpeed));
            }
            else
            {
                // Default gravity
                SetGravityScale(Data.gravityScale);
            }
        }
        else
        {
            // No gravity during dash
            SetGravityScale(0);
        }
        #endregion
    }

    private void FixedUpdate()
    {
        // Handle Run
        if (!IsDashing)
        {
            Run(1);
        }

        // Handle Wall Slide
        if (IsWallSliding)
            Slide();
    }

    #region INPUT CALLBACKS
    public void OnJumpInput()
    {
        LastPressedJumpTime = Data.jumpInputBufferTime;
    }

    public void OnJumpUpInput()
    {
        if (CanJumpCut() || CanWallJumpCut())
            _isJumpCut = true;
    }

    public void OnDashInput()
    {
        LastPressedDashTime = Data.dashInputBufferTime;
    }

    public void OnAttackInput()
    {
        LastPressedAttackTime = Data.attackInputBufferTime;
        // Attack logic can be added here
    }
    #endregion

    #region GENERAL METHODS
    public void SetGravityScale(float scale)
    {
        RB.gravityScale = scale;
    }

    private void Sleep(float duration)
    {
        StartCoroutine(nameof(PerformSleep), duration);
    }

    private IEnumerator PerformSleep(float duration)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1;
    }
    #endregion

    #region RUN METHODS
    private void Run(float lerpAmount)
    {
        float targetSpeed = _moveInput.x * Data.runMaxSpeed;
        targetSpeed = Mathf.Lerp(RB.linearVelocity.x, targetSpeed, lerpAmount);

        #region Calculate AccelRate
        float accelRate;

        if (LastOnGroundTime > 0)
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount : Data.runDeccelAmount;
        else
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount * Data.accelInAir : Data.runDeccelAmount * Data.deccelInAir;
        #endregion

        #region Add Bonus Jump Apex Acceleration
        // Floaty feel at apex
        if ((IsJumping || _isJumpFalling) && Mathf.Abs(RB.linearVelocity.y) < Data.jumpHangTimeThreshold)
        {
            accelRate *= Data.jumpHangAccelerationMult;
            targetSpeed *= Data.jumpHangMaxSpeedMult;
        }
        #endregion

        #region Conserve Momentum
        if (Data.doConserveMomentum && Mathf.Abs(RB.linearVelocity.x) > Mathf.Abs(targetSpeed) && 
            Mathf.Sign(RB.linearVelocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
        {
            accelRate = 0;
        }
        #endregion

        float speedDif = targetSpeed - RB.linearVelocity.x;
        float movement = speedDif * accelRate;

        RB.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }

    private void Turn()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        IsFacingRight = !IsFacingRight;
    }
    #endregion

    #region JUMP METHODS
    private void Jump()
    {
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;

        #region Perform Jump
        float force = Data.jumpForce;
        if (RB.linearVelocity.y < 0)
            force -= RB.linearVelocity.y;

        RB.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        #endregion

        // Play jump effect
        if (_jumpEffect != null)
            _jumpEffect.Play();
    }

    private void WallJump(int dir)
    {
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;
        LastOnWallRightTime = 0;
        LastOnWallLeftTime = 0;

        #region Perform Wall Jump
        Vector2 force = new Vector2(Data.wallJumpForce.x, Data.wallJumpForce.y);
        force.x *= dir;

        if (Mathf.Sign(RB.linearVelocity.x) != Mathf.Sign(force.x))
            force.x -= RB.linearVelocity.x;

        if (RB.linearVelocity.y < 0)
            force.y -= RB.linearVelocity.y;

        RB.AddForce(force, ForceMode2D.Impulse);
        #endregion

        // Auto-turn on wall jump (Hollow Knight behavior)
        if (Data.doTurnOnWallJump)
        {
            if ((dir == 1 && !IsFacingRight) || (dir == -1 && IsFacingRight))
                Turn();
        }

        // Play jump effect
        if (_jumpEffect != null)
            _jumpEffect.Play();
    }
    #endregion

    #region DASH METHODS
    private IEnumerator StartDash(Vector2 dir)
    {
        LastOnGroundTime = 0;
        LastPressedDashTime = 0;

        float startTime = Time.time;

        _dashesLeft--;
        _isDashAttacking = true;

        SetGravityScale(0);

        // Enable dash trail
        if (_dashTrail != null)
            _dashTrail.emitting = true;

        // Play dash effect
        if (_dashEffect != null)
            _dashEffect.Play();

        // Dash attack phase - maintain constant velocity
        while (Time.time - startTime <= Data.dashAttackTime)
        {
            RB.linearVelocity = dir.normalized * Data.dashSpeed;
            yield return null;
        }

        startTime = Time.time;
        _isDashAttacking = false;

        // Dash end phase - gradual slowdown
        SetGravityScale(Data.gravityScale);
        RB.linearVelocity = Data.dashEndSpeed * dir.normalized;

        while (Time.time - startTime <= Data.dashEndTime)
        {
            yield return null;
        }

        // Disable dash trail
        if (_dashTrail != null)
            _dashTrail.emitting = false;

        IsDashing = false;
    }

    private IEnumerator RefillDash(int amount)
    {
        _dashRefilling = true;
        yield return new WaitForSeconds(Data.dashRefillTime);
        _dashRefilling = false;
        _dashesLeft = Mathf.Min(Data.dashAmount, _dashesLeft + 1);
    }
    #endregion

    #region OTHER MOVEMENT METHODS
    private void Slide()
    {
        // Remove upward velocity when sliding
        if (RB.linearVelocity.y > 0)
        {
            RB.AddForce(-RB.linearVelocity.y * Vector2.up, ForceMode2D.Impulse);
        }

        float speedDif = Data.slideSpeed - RB.linearVelocity.y;
        float movement = speedDif * Data.slideAccel;
        movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

        RB.AddForce(movement * Vector2.up);
    }
    #endregion

    #region CHECK METHODS
    public void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != IsFacingRight)
            Turn();
    }

    private bool CanJump()
    {
        return LastOnGroundTime > 0 && !IsJumping;
    }

    private bool CanAirJump()
    {
        return _airJumpsLeft > 0 && LastOnGroundTime <= 0 && !IsJumping;
    }

    private bool CanWallJump()
    {
        return LastPressedJumpTime > 0 && LastOnWallTime > 0 && LastOnGroundTime <= 0;
    }

    private bool CanJumpCut()
    {
        return IsJumping && RB.linearVelocity.y > 0;
    }

    private bool CanWallJumpCut()
    {
        return RB.linearVelocity.y > 0;
    }

    private bool CanDash()
    {
        // Refill dash when touching ground
        if (!IsDashing && _dashesLeft < Data.dashAmount && LastOnGroundTime > 0 && !_dashRefilling)
        {
            StartCoroutine(nameof(RefillDash), 1);
        }

        return _dashesLeft > 0;
    }

    public bool CanWallSlide()
    {
        if (LastOnWallTime > 0 && !IsJumping && !IsDashing && LastOnGroundTime <= 0)
            return true;
        else
            return false;
    }
    #endregion

    #region EDITOR METHODS
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(_frontWallCheckPoint.position, _wallCheckSize);
        Gizmos.DrawWireCube(_backWallCheckPoint.position, _wallCheckSize);
    }
    #endregion
}
