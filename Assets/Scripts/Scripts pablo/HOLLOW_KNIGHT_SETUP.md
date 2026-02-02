# 🦋 HOLLOW KNIGHT MOVEMENT SYSTEM - GUÍA COMPLETA

## 📋 TABLA DE CONTENIDOS
1. [Setup Inicial](#setup-inicial)
2. [Configuración del Player](#configuración-del-player)
3. [Valores Recomendados](#valores-recomendados)
4. [Mecánicas Implementadas](#mecánicas-implementadas)
5. [Upgrades Progresivos](#upgrades-progresivos)
6. [Efectos Visuales Opcionales](#efectos-visuales-opcionales)

---

## 🎯 SETUP INICIAL

### 1. Crear el ScriptableObject de Datos

**Paso 1:** Click derecho en Project Window
```
Create → Hollow Knight → Player Data
```

**Paso 2:** Nombra el asset (ejemplo: "Knight_Data")

**Paso 3:** Ajusta los valores en el Inspector (ver sección Valores Recomendados)

---

## 🎮 CONFIGURACIÓN DEL PLAYER

### Componentes Necesarios en el GameObject:

#### 1. **Rigidbody2D** (Configuración crítica)
```
Body Type: Dynamic
Material: None (o crea uno con Friction = 0)
Simulated: ✓
Use Auto Mass: ✗
Mass: 1
Linear Drag: 0
Angular Drag: 0
Gravity Scale: 3 (se ajusta automáticamente por el script)
Collision Detection: Continuous
Sleeping Mode: Never Sleep
Interpolate: Interpolate (para movimiento suave)
Constraints:
  - Freeze Position: ✗ ✗
  - Freeze Rotation: ✗ ✗ ✓ (solo Z)
```

#### 2. **Collider2D** 
```
Preferiblemente CapsuleCollider2D:
- Size: Ajusta al sprite del personaje
- Offset: Centra según tu sprite
- Material: Physics Material 2D con Friction = 0
```

#### 3. **SpriteRenderer**
```
- Sprite: Tu sprite del Knight
- Sorting Layer: Player
- Order in Layer: 0
```

#### 4. **Animator** (opcional pero recomendado)
```
- Controller: Tu Animation Controller
```

#### 5. **HollowKnightMovement Script**
```
- Adjunta al GameObject
- Arrastra el HollowKnightData al campo "Data"
```

---

### GameObjects Hijo (Children):

#### **GroundCheck** (Empty GameObject)
```
Posición local: (0, -0.5, 0) // Ajusta según altura de tu sprite
Parent: Player
```

#### **FrontWallCheck** (Empty GameObject)
```
Posición local: (0.3, 0, 0) // Ajusta según ancho de tu sprite
Parent: Player
```

#### **BackWallCheck** (Empty GameObject)
```
Posición local: (-0.3, 0, 0) // Ajusta según ancho de tu sprite
Parent: Player
```

---

## 📊 VALORES RECOMENDADOS

### 🎮 **Configuración Base (Early Game - Sin Upgrades)**

```csharp
// GRAVITY
fallGravityMult = 2.5f
maxFallSpeed = 18f
fastFallGravityMult = 3.5f
maxFastFallSpeed = 25f

// RUN
runMaxSpeed = 6f
runAcceleration = 4f
runDecceleration = 4.5f
accelInAir = 0.7f
deccelInAir = 0.75f
doConserveMomentum = true

// JUMP
jumpHeight = 4.2f
jumpTimeToApex = 0.45f
jumpCutGravityMult = 2.2f
jumpHangGravityMult = 0.4f      // Floaty feeling!
jumpHangTimeThreshold = 2.5f
jumpHangAccelerationMult = 1.15f
jumpHangMaxSpeedMult = 1.1f

// AIR JUMPS (Monarch Wings)
airJumpsAmount = 0  // ← Sin doble salto al inicio

// WALL
wallJumpForce = (12, 16)
wallJumpRunLerp = 0.3f
wallJumpTime = 0.15f
doTurnOnWallJump = true
slideSpeed = -2.5f
slideAccel = 30f

// DASH (Mothwing Cloak)
dashAmount = 1
dashSpeed = 18f
dashSleepTime = 0.02f
dashAttackTime = 0.18f
dashEndTime = 0.22f
dashEndSpeed = (8, 8)
dashEndRunLerp = 0.6f
dashRefillTime = 0.05f

// ASSISTS
coyoteTime = 0.12f
jumpInputBufferTime = 0.1f
dashInputBufferTime = 0.1f
```

---

### 🔧 **Variantes de Configuración**

#### **Modo Speedrun (Más rápido, menos floaty)**
```csharp
runMaxSpeed = 7.5f
jumpHeight = 4f
jumpTimeToApex = 0.4f
jumpHangGravityMult = 0.6f  // Menos flotante
dashSpeed = 22f
```

#### **Modo Principiante (Más control, más assist)**
```csharp
runMaxSpeed = 5f
jumpHeight = 4.5f
coyoteTime = 0.15f
jumpInputBufferTime = 0.15f
airJumpsAmount = 1  // Doble salto desde inicio
```

#### **Modo Hard (Como el verdadero HK)**
```csharp
coyoteTime = 0.08f
jumpInputBufferTime = 0.08f
airJumpsAmount = 0
dashAmount = 1
```

---

## ✨ MECÁNICAS IMPLEMENTADAS

### ✅ **Movimiento Base**
- [x] Aceleración/desaceleración suave
- [x] Diferente aceleración en aire vs suelo
- [x] Conservación de momento del dash
- [x] Auto-flip del sprite según dirección

### ✅ **Sistema de Salto**
- [x] Salto de altura variable (mantener/soltar botón)
- [x] Coyote Time (saltar después de caer del borde)
- [x] Jump Buffer (presionar antes de tocar suelo)
- [x] Jump Hang (sensación flotante en el apex)
- [x] Física mejorada (caída más pesada)
- [x] Fast Fall (mantener ↓ mientras caes)

### ✅ **Air Jumps (Monarch Wings)**
- [x] Doble salto configurable
- [x] Se resetea al tocar suelo
- [x] Sistema preparado para triple salto (mods)

### ✅ **Wall Mechanics**
- [x] Wall Slide suave
- [x] Wall Jump direccional
- [x] Auto-turn hacia dirección del wall jump
- [x] Detección dual (front/back wall check)

### ✅ **Dash (Mothwing Cloak)**
- [x] Dash en 8 direcciones
- [x] Dash horizontal por defecto (sin input direccional)
- [x] Sistema de dos fases (attack + end)
- [x] Cancela gravedad durante dash
- [x] Recarga automática al tocar suelo
- [x] Preparado para Shade Cloak (doble dash)

### ✅ **Gravity System**
- [x] 6 estados diferentes de gravedad
- [x] Sin gravedad en wall slide
- [x] Más gravedad al caer
- [x] Fast fall al presionar ↓
- [x] Jump cut (soltar botón)
- [x] Jump hang (apex del salto)

---

## 🎁 UPGRADES PROGRESIVOS

### Sistema de Progresión (Como el juego original)

#### **1. Mothwing Cloak (Dash básico)**
```csharp
// Al inicio del juego
Data.dashAmount = 0;  // Sin dash

// Cuando obtienes el upgrade
Data.dashAmount = 1;  // ¡Ahora puedes dashear!
```

#### **2. Monarch Wings (Doble Salto)**
```csharp
// Al inicio
Data.airJumpsAmount = 0;  // Sin doble salto

// Al obtener Monarch Wings
Data.airJumpsAmount = 1;  // ¡Doble salto desbloqueado!
```

#### **3. Shade Cloak (Dash mejorado)**
```csharp
// Con Mothwing Cloak
Data.dashAmount = 1;

// Al mejorar a Shade Cloak
Data.dashAmount = 2;  // ¡Doble dash!
Data.dashSpeed = 20f;  // Un poco más rápido
```

#### **4. Crystal Heart (Dash súper - futuro)**
```csharp
// Puede agregarse como mecánica especial
// Requiere mantener el botón de dash
```

#### **Ejemplo de sistema de upgrades:**
```csharp
public class UpgradeManager : MonoBehaviour
{
    public HollowKnightData knightData;
    
    public void UnlockMothwingCloak()
    {
        knightData.dashAmount = 1;
        Debug.Log("¡Mothwing Cloak obtenida!");
    }
    
    public void UnlockMonarchWings()
    {
        knightData.airJumpsAmount = 1;
        Debug.Log("¡Monarch Wings obtenidas!");
    }
    
    public void UnlockShadeCloak()
    {
        knightData.dashAmount = 2;
        knightData.dashSpeed = 20f;
        Debug.Log("¡Shade Cloak obtenida!");
    }
}
```

---

## 🎨 EFECTOS VISUALES OPCIONALES

### Particle Systems (Mejoran el juego feel)

#### **1. Dash Effect**
```
GameObject: DashEffect
Component: Particle System
Settings:
  - Duration: 0.3
  - Start Lifetime: 0.2
  - Start Speed: 5
  - Start Size: 0.3
  - Shape: Cone, Angle: 15
  - Emission: Burst = 10 particles
```

#### **2. Jump Effect**
```
GameObject: JumpEffect
Component: Particle System
Settings:
  - Duration: 0.2
  - Start Lifetime: 0.3
  - Start Speed: 2-4
  - Shape: Circle, Radius: 0.2
  - Emission: Burst = 5 particles
```

#### **3. Land Effect**
```
GameObject: LandEffect
Component: Particle System
Settings:
  - Duration: 0.2
  - Start Lifetime: 0.3
  - Start Speed: 1-3
  - Shape: Hemisphere
  - Emission: Burst = 8 particles
```

#### **4. Dash Trail**
```
GameObject: Player
Component: Trail Renderer
Settings:
  - Time: 0.2
  - Width: 0.3 → 0
  - Color: Blanco con alpha gradient
  - Material: Sprites/Default
```

### Cómo conectar los efectos:

```csharp
// En el Inspector del Player, arrastra:
[SerializeField] private ParticleSystem _dashEffect;   // → DashEffect
[SerializeField] private ParticleSystem _jumpEffect;   // → JumpEffect  
[SerializeField] private ParticleSystem _landEffect;   // → LandEffect
[SerializeField] private TrailRenderer _dashTrail;     // → Trail Renderer component
```

---

## 🎮 CONTROLES

### Input por defecto:

```
MOVIMIENTO:
← → : A/D o Flechas
↓   : S o Flecha Abajo (fast fall)

SALTO:
Space, C, J, W, ↑

DASH:
LeftShift, X, K

ATAQUE (futuro):
Z, Mouse0
```

### Input direccional del Dash:

```
→ + Dash = Dash derecha
← + Dash = Dash izquierda
↑ + Dash = Dash arriba
↓ + Dash = Dash abajo
↗ + Dash = Dash diagonal arriba-derecha
etc... (8 direcciones totales)

Sin input = Dash hacia donde miras
```

---

## 🔍 DEBUGGING

### Gizmos en Scene View:
- **Verde**: Ground Check (donde detecta suelo)
- **Azul**: Wall Checks (donde detecta paredes)

### Tips de debug:
1. Si el salto se siente raro → Ajusta `jumpHeight` y `jumpTimeToApex`
2. Si cae muy rápido → Reduce `fallGravityMult`
3. Si el dash no se siente bien → Ajusta `dashSpeed` y `dashAttackTime`
4. Si el wall slide es muy rápido → Reduce `slideSpeed` (más negativo)
5. Si el control en aire es malo → Aumenta `accelInAir`

---

## 🎯 DIFERENCIAS CLAVE CON HOLLOW KNIGHT ORIGINAL

### Lo que ESTÁ implementado:
✅ Movimiento base exacto
✅ Dash en 8 direcciones
✅ Wall mechanics completas
✅ Jump feel (el floaty característico)
✅ Sistema de upgrades progresivos
✅ Doble salto (Monarch Wings)
✅ Doble dash (Shade Cloak ready)

### Lo que FALTA (para implementar después):
❌ Pogo/Downward attack (rebotar en enemigos)
❌ Crystal Heart (super dash horizontal infinito)
❌ Isma's Tear (nadar en ácido)
❌ Vengeful Spirit (proyectil mágico)
❌ Nail attacks (sistema de combate)
❌ Focus (curación)

---

## 💡 TIPS PARA MEJORAR EL GAME FEEL

### 1. **Añadir Screen Shake en el dash**
```csharp
// Añadir al StartDash()
CameraShake.Shake(0.1f, 0.1f);
```

### 2. **Freeze Frame más notorio**
```csharp
Data.dashSleepTime = 0.05f;  // Un poco más largo
```

### 3. **Partículas de polvo al correr**
```csharp
// Crear sistema que emita mientras corre en suelo
```

### 4. **SFX críticos**
- Salto: Sonido sutil "whoosh"
- Dash: Sonido distintivo "swoosh"
- Land: Sonido de impacto suave
- Wall Jump: Combinación de salto + impacto

---

## 🚀 PRÓXIMOS PASOS

1. **Implementar el sistema básico** siguiendo esta guía
2. **Testear y ajustar valores** según tu preferencia
3. **Añadir efectos visuales** para mejorar el feedback
4. **Implementar sistema de upgrades** progresivos
5. **Añadir combate** (nail attacks, spells)
6. **Crear enemigos** con interacción de pogo

---

## 📝 NOTAS FINALES

Este sistema está diseñado para capturar la **esencia del movimiento de Hollow Knight**:

- **Pesado pero responsive**: Se siente como un caballero en armadura, pero con control preciso
- **Floaty en el aire**: El jump hang da esa sensación característica
- **Dash potente**: El dash se siente impactante y útil
- **Wall mechanics fluidas**: Wall slide y wall jump funcionan intuitivamente
- **Progresión satisfactoria**: Los upgrades se sienten como mejoras reales

**¡Buena suerte con tu proyecto!** 🦋⚔️
