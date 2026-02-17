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

    //Attack
    private float _attackStartTime;
    private Vector2 _attackDirection;
    #endregion
    
    //Health
    public int CurrentHealth { get; private set; }
    public int MaxHealth { get; private set; }
    public bool IsInvulnerable { get; private set; }
    private float _invulnerabilityTimer;
    private float _fallSpeedYDampingChangeThreshold;
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
    
       // Initialize Health 
        MaxHealth = Data.maxHealth;
        CurrentHealth = MaxHealth;
        IsInvulnerable = false;
        _fallSpeedYDampingChangeThreshold= CameraManager.instance._fallSpeedYDampingChangeThreshold;
    }

    private void Update()
    {
        #region CAMERA MANAGER
        if (RB.linearVelocity.y < _fallSpeedYDampingChangeThreshold && !CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpYDamping(true);
            CameraManager.instance.LerpedFromPlayerFalling = true;
        }
        else if (RB.linearVelocity.y >= _fallSpeedYDampingChangeThreshold && CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpYDamping(false);
            CameraManager.instance.LerpedFromPlayerFalling = false;
        }
        #endregion

        #region TIMERS
        LastOnGroundTime -= Time.deltaTime;
        LastOnWallTime -= Time.deltaTime;
        LastOnWallRightTime -= Time.deltaTime;
        LastOnWallLeftTime -= Time.deltaTime;

        LastPressedJumpTime -= Time.deltaTime;
        LastPressedDashTime -= Time.deltaTime;
        LastPressedAttackTime -= Time.deltaTime;
            // Invulnerability Timer 
        if (IsInvulnerable)
        {
            _invulnerabilityTimer -= Time.deltaTime;
            if (_invulnerabilityTimer <= 0)
            {
                IsInvulnerable = false;
                // Restaurar opacidad del sprite
                if (spriteRenderer != null)
                {
                    Color c = spriteRenderer.color;
                    c.a = 1f;
                    spriteRenderer.color = c;
                }
            }
            else
            {
                // Efecto de parpadeo durante invulnerabilidad
                float alpha = Mathf.PingPong(Time.time * 10f, 1f);
                if (spriteRenderer != null)
                {
                    Color c = spriteRenderer.color;
                    c.a = alpha;
                    spriteRenderer.color = c;
                }
            }
        }
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
        // TESTING - Recibir daño (TEMPORAL)  - Presiona H para simular daño desde la izquierda
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(1, transform.position + Vector3.left);
            Debug.Log("TEST: Daño recibido desde la izquierda");
        }
        
        // TESTING - Curar (TEMPORAL) - Presiona G para curar 1 punto de vida
        if (Input.GetKeyDown(KeyCode.G))
        {
            Heal(1);
            Debug.Log("TEST: Curado");
        }
        #endregion

        #region COLLISION CHECKS
        if (!IsDashing && !IsJumping)
        {
            // Ground Check (cache OverlapBox results to avoid duplicate physics calls)
            bool wasGrounded = LastOnGroundTime > 0;
            bool isGrounded = Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer);
            if (isGrounded)
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

            // Wall checks: cache front/back
            bool frontWall = Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer);
            bool backWall = Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer);

            // Right Wall Check
            if ((frontWall && IsFacingRight) || (backWall && !IsFacingRight))
                LastOnWallRightTime = Data.coyoteTime;

            // Left Wall Check
            if ((frontWall && !IsFacingRight) || (backWall && IsFacingRight))
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

            // Use IEnumerator overload (type-safe)
            StartCoroutine(StartDash(_lastDashDir));
        }
        #endregion

        #region ATTACK CHECKS  
        if (CanAttack() && LastPressedAttackTime > 0)
        {
            DetermineAttackDirection();
            IsAttacking = true;
            _attackStartTime = Time.time;
            StartCoroutine(PerformAttack());
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
                SetGravityScale(Data.gravityScale*0.3f);
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
        // Avoid setting gravityScale every frame if unchanged
        if (!Mathf.Approximately(RB.gravityScale, scale))
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
        // Cache current velocity to minimize repeated property access
        float currentVelX = RB.linearVelocity.x;
        float currentVelY = RB.linearVelocity.y;

        float targetSpeed = _moveInput.x * Data.runMaxSpeed;
        targetSpeed = Mathf.Lerp(currentVelX, targetSpeed, lerpAmount);

        #region Calculate AccelRate
        float absTarget = Mathf.Abs(targetSpeed);
        float accelRate;

        if (LastOnGroundTime > 0)
            accelRate = (absTarget > 0.01f) ? Data.runAccelAmount : Data.runDeccelAmount;
        else
            accelRate = (absTarget > 0.01f) ? Data.runAccelAmount * Data.accelInAir : Data.runDeccelAmount * Data.deccelInAir;
        #endregion

        #region Add Bonus Jump Apex Acceleration
        // Floaty feel at apex
        if ((IsJumping || _isJumpFalling) && Mathf.Abs(currentVelY) < Data.jumpHangTimeThreshold)
        {
            accelRate *= Data.jumpHangAccelerationMult;
            targetSpeed *= Data.jumpHangMaxSpeedMult;
        }
        #endregion

        #region Conserve Momentum
        if (Data.doConserveMomentum && Mathf.Abs(currentVelX) > absTarget && 
            Mathf.Sign(currentVelX) == Mathf.Sign(targetSpeed) && absTarget > 0.01f && LastOnGroundTime < 0)
        {
            accelRate = 0;
        }
        #endregion

        float speedDif = targetSpeed - currentVelX;
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
        return LastPressedJumpTime > 0 && LastOnWallTime > 0 && LastOnGroundTime <= 0 && !IsJumping;
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
            StartCoroutine(RefillDash(1));
        }

        return _dashesLeft > 0;
    }

    public bool CanWallSlide()
    {
        return (LastOnWallTime > 0 && !IsJumping && !IsDashing && LastOnGroundTime <= 0);
    }

    private bool CanAttack()
    {
        // No atacar durante dash o si ya estás atacando
        if (IsDashing || IsAttacking)
            return false;

        return true;
    }
    #endregion


    #region ATTACK METHODS 
    private void DetermineAttackDirection()
    {
        // Determinar dirección del ataque basado en input (4 direcciones como HK)
        
        // PRIORIDAD: Arriba y Abajo tienen prioridad sobre horizontal
        if (_moveInput.y > 0.5f)
        {
            // Ataque ARRIBA
            _attackDirection = Vector2.up;
            Debug.Log("Ataque ARRIBA");
        }
        else if (_moveInput.y < -0.5f)
        {
            // Ataque ABAJO (pogo en HK)
            _attackDirection = Vector2.down;
            Debug.Log("Ataque ABAJO (Pogo)");
        }
        else
        {
            // Ataque LATERAL (derecha o izquierda según donde mires)
            _attackDirection = IsFacingRight ? Vector2.right : Vector2.left;
            Debug.Log($"Ataque LATERAL ({(IsFacingRight ? "Derecha" : "Izquierda")})");
        }
    }

   private IEnumerator PerformAttack()
{
    LastPressedAttackTime = 0;
    
    // Pequeño impulso en la dirección del ataque (como en HK)
    if (_attackDirection == Vector2.down && !IsGrounded())
    {
        // Pogo: Pequeño impulso hacia arriba al atacar abajo en el aire
        RB.linearVelocity = new Vector2(RB.linearVelocity.x, 0f);
        RB.AddForce(Vector2.up * Data.jumpForce * 0.5f, ForceMode2D.Impulse);
        Debug.Log("¡POGO! Rebote hacia arriba");
    }
    
    // Aquí irían las animaciones
    // AnimHandler.SetTrigger("Attack_" + GetAttackDirectionString());
    
    // Esperar un frame para que la animación empiece
    yield return new WaitForSeconds(0.1f);
    
    // DETECTAR Y GOLPEAR ENEMIGOS
    DetectAndHitEnemies();
    
    // Duración del resto del ataque
    float remainingDuration = Data.attackDuration - 0.1f;
    yield return new WaitForSeconds(remainingDuration);
    
    IsAttacking = false;
}
private void DetectAndHitEnemies()
{
    // Calcular posición del hitbox según dirección del ataque
    Vector2 attackPosition = (Vector2)transform.position + (_attackDirection * Data.attackRange);
    
    // Detectar todos los colliders en el área de ataque
    Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(
        attackPosition, 
        Data.attackHitboxSize, 
        0f, 
        Data.enemyLayer  // Necesitamos añadir esto al Data
    );
    
    Debug.Log($"Enemigos detectados: {hitEnemies.Length}");
    
    // Aplicar daño a cada enemigo golpeado
    foreach (Collider2D enemy in hitEnemies)
    {
        // Intentar obtener el componente de salud del enemigo
        EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
        
        if (enemyHealth != null)
        {
            // Calcular dirección del knockback (desde el jugador hacia el enemigo)
            Vector2 knockbackDirection = (enemy.transform.position - transform.position).normalized;
            
            // Aplicar daño al enemigo
            enemyHealth.TakeDamage(Data.attackDamage, knockbackDirection);
            
            Debug.Log($"¡Golpeaste a {enemy.name}!");
            
            // Pogo extra si golpeaste hacia abajo en el aire
            if (_attackDirection == Vector2.down && !IsGrounded())
            {
                // Impulso extra al golpear enemigo (pogo mejorado)
                RB.linearVelocity = new Vector2(RB.linearVelocity.x, 0f);
                RB.AddForce(Vector2.up * Data.jumpForce * 0.7f, ForceMode2D.Impulse);
                Debug.Log("¡POGO MEJORADO por golpear enemigo!");
            }
        }
    }
}

    private string GetAttackDirectionString()
    {
        if (_attackDirection == Vector2.up) return "Up";
        if (_attackDirection == Vector2.down) return "Down";
        if (_attackDirection == Vector2.right) return "Right";
        return "Left";
    }

    private bool IsGrounded()
    {
        return LastOnGroundTime > 0;
    }
    #endregion

    #region HEALTH METHODS 
    public void TakeDamage(int damage, Vector2 damageSourcePosition)
    {
        // No recibir daño si estamos invulnerables o muertos
        if (IsInvulnerable || CurrentHealth <= 0)
            return;
        
        // Reducir vida
        CurrentHealth -= damage;
        CurrentHealth = Mathf.Max(CurrentHealth, 0);
        
        Debug.Log($"¡Daño recibido! Vida: {CurrentHealth}/{MaxHealth}");
        
        // Activar invulnerabilidad
        IsInvulnerable = true;
        _invulnerabilityTimer = Data.invulnerabilityDuration;
        
        // Aplicar knockback
        ApplyKnockback(damageSourcePosition);
        
        // Efectos visuales/sonido aquí
        // PlayHurtSound();
        // PlayHurtAnimation();

        // Verificar muerte
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }
    
    private void ApplyKnockback(Vector2 damageSourcePosition)
    {
        // Calcular dirección del knockback (alejarse de la fuente de daño)
        Vector2 knockbackDirection = ((Vector2)transform.position - damageSourcePosition).normalized;
        
        // Cancelar velocidad actual
        RB.linearVelocity = Vector2.zero;
        
        // Aplicar fuerza de knockback
        Vector2 knockbackForce = new Vector2(
            knockbackDirection.x * Data.knockbackForce.x,
            Data.knockbackForce.y  // Siempre empuja hacia arriba
        );
        
        RB.AddForce(knockbackForce, ForceMode2D.Impulse);
        
        Debug.Log($"Knockback aplicado: {knockbackForce}");
    }
    
    public void Heal(int amount)
    {
        if (CurrentHealth >= MaxHealth)
            return;
        
        CurrentHealth += amount;
        CurrentHealth = Mathf.Min(CurrentHealth, MaxHealth);
        
        Debug.Log($"¡Curado! Vida: {CurrentHealth}/{MaxHealth}");
        
        // Efectos visuales/sonido aquí
        // PlayHealSound();
        // PlayHealParticles();
    }
    
    private void Die()
    {
        Debug.Log("¡Has muerto!");
        
        // Detener movimiento
        RB.linearVelocity = Vector2.zero;
        
        // Desactivar controles (opcional)
        // enabled = false;
        
        // Animación de muerte
        // AnimHandler.SetTrigger("Death");
        
        // Esperar y respawnear
        StartCoroutine(nameof(RespawnAfterDelay));
    }
    
    private IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(Data.respawnDelay);
        
        Respawn();
    }
    
    private void Respawn()
    {
        // Restaurar vida
        CurrentHealth = MaxHealth;
        IsInvulnerable = true;
        _invulnerabilityTimer = Data.invulnerabilityDuration;
        
        // Teleport al último checkpoint (por ahora, posición inicial)
        if (Data.respawnPoint != null)
        {
            transform.position = Data.respawnPoint.position;
        }
        else
        {
            // Si no hay checkpoint, respawn en posición actual + arriba
            transform.position = new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z);
        }
        
        // Resetear velocidad
        RB.linearVelocity = Vector2.zero;
        
        // Restaurar controles
        // enabled = true;
        
        Debug.Log("¡Respawneado!");
    }
    
    public void SetRespawnPoint(Transform newRespawnPoint)
    {
        Data.respawnPoint = newRespawnPoint;
        Debug.Log($"Nuevo punto de respawn: {newRespawnPoint.position}");
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
        if (Application.isPlaying && IsAttacking)
    {
        Gizmos.color = Color.red;
        Vector2 attackPosition = (Vector2)transform.position + (_attackDirection * Data.attackRange);
        Gizmos.DrawWireCube(attackPosition, Data.attackHitboxSize);
    }

    }
    #endregion
}
