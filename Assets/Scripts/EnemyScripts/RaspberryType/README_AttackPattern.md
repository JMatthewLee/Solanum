# Raspberry & Blackberry Enemy Attack Pattern System

## Overview
The new attack pattern system implements a strategic, state-based approach for raspberry and blackberry enemies that makes them more predictable and tactical rather than directly following the player.

## Key Features

### 1. Limited Attack Engagement
- **Maximum 2 attacks per engagement**: Enemies will attack up to 2 times before repositioning
- **Attack counter resets** when the player moves too far away (beyond `maxAttackDistance`)

### 2. Strategic Positioning
- **No direct following**: Enemies don't chase the player directly
- **Strategic angles**: Enemies position themselves at strategic angles relative to the player
- **Path-based movement**: Movement considers the enemy's shooting pattern and optimal attack angles

### 3. State Machine
The enemy operates in five distinct states:

#### Patrolling
- **Independent movement** around the map within patrol area
- **Not chasing player** - moves to random points within `patrolRadius`
- **Smart transitions** to attack when player is in range and well-aligned
- **Time-based movement** - changes patrol points every `patrolPointChangeTime` seconds

#### Approaching
- Enemy moves toward the player to get within `attackRange`
- Uses strategic positioning to approach from optimal angles
- Avoids direct collision paths

#### Attacking
- Enemy stops moving and performs attack animation
- Attack logic is handled by animation events
- State transitions based on attack completion and distance

#### Immediate Attack
- **Priority response** for very close threats (within `immediateAttackRange`)
- **Bypasses normal positioning** for immediate defense
- **Stops all movement** and attacks immediately

#### Repositioning
- After max attacks or if too far, enemy moves to new strategic position
- Spends `repositionTime` seconds repositioning
- Calculates new position at different angle from player

#### Returning
- Enemy moves back toward player for next engagement
- Uses strategic positioning for optimal approach angle

## Configuration Parameters

### Attack Ranges
- **`attackRange`**: Distance to initiate first attack
- **`attackContinuationRange`**: Range to continue attacking (larger than attackRange)
- **`repositionRange`**: Range where enemy will reposition instead of attacking
- **`maxAttackDistance`**: Maximum distance before enemy gives up and moves

### Attack Behavior
- **`maxAttacksPerEngagement`**: Maximum attacks before repositioning (default: 2)
- **`repositionTime`**: Time spent repositioning (default: 3 seconds)
- **`attackDelay`**: Delay between attacks

### Strategic Positioning
- **`strategicPositioningRadius`**: Radius for strategic positioning around player
- **`chaseRange`**: Range at which enemy starts chasing player

### Movement Control
- **`movementSpeedMultiplier`**: Multiplier for faster enemy movement (default: 1.5x)
- **`stopThreshold`**: Distance threshold to consider enemy stopped (default: 0.1)
- **`immediateAttackRange`**: Very close range for immediate attack response (default: 2.0)
- **`landingCommitTime`**: Time to commit to attack after landing (default: 0.5 seconds)
- **`blueberrySpeedModifier`**: Additional speed reduction for blueberry enemies (default: 0.8 = 80% speed)
- **`isBlueberryEnemy`**: Manual override to identify blueberry enemies (check this box for blueberry enemies)

### Patrol System
- **`usePatrolMode`**: Enable independent patrol movement around the map (default: true)
- **`useNodeBasedPatrol`**: Use predefined patrol nodes instead of random movement (default: true)
- **`patrolNodes`**: Array of Transform objects defining the patrol path
- **`nodeReachThreshold`**: Distance to consider a node reached (default: 0.5)
- **`nodeWaitTime`**: Time to wait at each node before moving to next (default: 2.0 seconds)
- **`maxDistanceFromNodes`**: Maximum distance enemy can stray from nodes (default: 6.0)
- **`tacticalNodeRadius`**: Radius around nodes for tactical positioning when player spotted (default: 4.0)

### Projectile Alignment
- **`projectileAlignmentRadius`**: Radius for positioning to align projectiles with player (default: 3.0)
- **`alignmentTolerance`**: Degrees tolerance for projectile alignment (default: 15° = ±15° from perfect alignment)

## How It Works

1. **Node-Based Patrolling**: Enemy follows predefined patrol nodes in sequence, creating predictable routes
2. **Tactical Node Positioning**: When player is spotted, enemy stays near current node for tactical advantage
3. **Smart Attack Transitions**: When player enters attack range, enemy checks if it's well-aligned for projectiles
4. **Immediate Threat Response**: If player is very close (`immediateAttackRange`), enemy attacks immediately
5. **Projectile Alignment**: Enemy positions itself at 8-directional angles (0°, 45°, 90°, 135°, 180°, 225°, 270°, 315°) relative to player for optimal projectile accuracy
6. **Landing Commitment**: Once enemy lands/stops, they commit to attack after `landingCommitTime`
7. **Attack Execution**: When within `attackRange` and well-aligned, enemy attacks
8. **Decision Making**: After attack, enemy evaluates:
   - If max attacks reached → Return to patrol mode or reposition
   - If too far → Return to patrol mode or reposition
   - If still in range → Attack again from current position
9. **Return to Patrol**: Enemy resumes following the predefined node path

## Benefits

- **More predictable**: Players can learn enemy attack patterns
- **Tactical gameplay**: Enemies don't just chase mindlessly
- **Better AI**: Projectile alignment creates more accurate and threatening encounters
- **Performance**: Limited attacks prevent endless engagement
- **Variety**: Different alignment angles create varied attack patterns
- **Faster movement**: Enemies move 1.5x faster for more dynamic combat
- **Precise stopping**: Enemies stop completely when attacking, no more drifting
- **Immediate threat response**: Enemies attack instantly when player gets too close
- **Landing commitment**: Enemies commit to attacks once they land/stop moving
- **Blueberry-specific speed**: Blueberry enemies are slower than the player for balanced gameplay
- **Projectile accuracy**: Enemies position themselves for maximum projectile hit chance
- **Strategic positioning**: 8-directional alignment system ensures projectiles can actually reach the player
- **Predictable patrol routes**: Node-based system creates consistent enemy movement patterns
- **Tactical positioning**: Enemies stay near nodes when player is spotted for better attack angles
- **Controlled aggression**: Enemies don't get overly distracted, maintaining their patrol discipline
- **Map coverage**: Enemies cover specific areas defined by nodes, preventing camping in safe zones
- **Lethal Company style**: Similar to the popular game's enemy AI behavior

## Usage

1. **Configure parameters** in the Inspector using the organized sections
2. **Test different values** for ranges and timing
3. **Adjust strategic positioning radius** to match your level design
4. **Fine-tune attack limits** based on desired difficulty

## Technical Notes

- Uses NavMeshAgent for pathfinding
- Implements fallback positioning if strategic positions are unreachable
- State transitions are handled in `FixedUpdate` for consistent timing
- Strategic positioning considers current enemy position relative to player
- All movement uses the base class's improved positioning methods
