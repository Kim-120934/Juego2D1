using UnityEngine;

[CreateAssetMenu(menuName = "Hollow Knight/Player Data")]
public class HollowKnightData : ScriptableObject
{
    [Header("Gravity")]
    [HideInInspector] public float gravityStrength;
    [HideInInspector] public float gravityScale;
    [Space(5)]
    public float fallGravityMult = 2.5f;
    public float maxFallSpeed = 18f;
    [Space(5)]
    public float fastFallGravityMult = 3.5f;
    public float maxFastFallSpeed = 25f;

    [Space(20)]

    [Header("Run")]
    public float runMaxSpeed = 6f;
    public float runAcceleration = 4f;
    [HideInInspector] public float runAccelAmount;
    public float runDecceleration = 4.5f;
    [HideInInspector] public float runDeccelAmount;
    [Space(5)]
    [Range(0f, 1f)] public float accelInAir = 0.7f;
    [Range(0f, 1f)] public float deccelInAir = 0.75f;
    [Space(5)]
    public bool doConserveMomentum = true;

    [Space(20)]

    [Header("Jump")]
    public float jumpHeight = 4.2f;
    public float jumpTimeToApex = 0.45f;
    [HideInInspector] public float jumpForce;

    [Header("Jump Feel")]
    public float jumpCutGravityMult = 2.2f;
    [Range(0f, 1f)] public float jumpHangGravityMult = 0.4f;
    public float jumpHangTimeThreshold = 2.5f;
    [Space(0.5f)]
    public float jumpHangAccelerationMult = 1.15f;
    public float jumpHangMaxSpeedMult = 1.1f;

    [Header("Air Jumps (Monarch Wings)")]
    [Tooltip("0 = No air jumps, 1 = Double jump (default Hollow Knight), 2 = Triple jump")]
    public int airJumpsAmount = 1;

    [Header("Wall Jump")]
    public Vector2 wallJumpForce = new Vector2(12f, 16f);
    [Space(5)]
    [Range(0f, 1f)] public float wallJumpRunLerp = 0.3f;
    [Range(0f, 1.5f)] public float wallJumpTime = 0.15f;
    public bool doTurnOnWallJump = true;

    [Space(20)]

    [Header("Wall Slide")]
    public float slideSpeed = -1.5f;
    public float slideAccel = 20f;

    [Header("Assists")]
    [Range(0.01f, 0.5f)] public float coyoteTime = 0.12f;
    [Range(0.01f, 0.5f)] public float jumpInputBufferTime = 0.1f;

    [Space(20)]

    [Header("Dash (Mothwing Cloak)")]
    [Tooltip("1 = Single dash, 2 = Double dash (Shade Cloak upgrade)")]
    public int dashAmount = 1;
    public float dashSpeed = 18f;
    public float dashSleepTime = 0.02f;
    [Space(5)]
    public float dashAttackTime = 0.18f;
    [Space(5)]
    public float dashEndTime = 0.22f;
    public Vector2 dashEndSpeed = new Vector2(8f, 8f);
    [Range(0f, 1f)] public float dashEndRunLerp = 0.6f;
    [Space(5)]
    public float dashRefillTime = 0.05f;
    [Space(5)]
    [Range(0.01f, 0.5f)] public float dashInputBufferTime = 0.1f;

    [Space(20)]

    [Header("Attack (For future implementation)")]
    [Range(0.01f, 0.5f)] public float attackInputBufferTime = 0.1f;

    private void OnValidate()
    {
        // Calculate gravity using physics formulas
        gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);
        gravityScale = gravityStrength / Physics2D.gravity.y;

        // Calculate acceleration amounts
        runAccelAmount = (50 * runAcceleration) / runMaxSpeed;
        runDeccelAmount = (50 * runDecceleration) / runMaxSpeed;

        // Calculate jump force
        jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;

        #region Variable Ranges
        runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, runMaxSpeed);
        runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, runMaxSpeed);
        #endregion
    }
}
