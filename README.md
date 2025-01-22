# TheProject Unity

## Fonctionnalités
- Ce jeu est inspiré de Totally Accurate Battle Simulator
- Les attacks ne sont déclenchés que si on est sûre de toucher l'Ennemy

## Classes communes

### Entity
- `String name`
- `int price`
- `# GetName`
- `# GetPrice`

### Item
- `Entity entity`
- `List<Attribute, float> upgrades`
- `# runUpgrades`

### Abstract Attribute
- `# Upgrade (int)`

### Champion
- `Entity entity`
- `Health health`
- `List<Item> items`
- `Attack attack`
- `Mouvement mouvement`

### Mouvement
- `Attribute Attribute`
- `# Upgrade`
- `float speed`
- `List<float> speedMultipliers`
- `# GetSpeed`
- `# SetSpeedMultiplier`
- `# GetSpeedMultipliers`

### Attack
- `Attribute Attribute`
- `# Upgrade`
- `float cooldown` (0.2s–1.5s)
- `float distance`
- `float damage`
- `# GetDamage`
- `# SetDamage`

### Health
- `Attribute Attribute`
- `# Upgrade`
- `float health`
- `# TakeDamage`
- `# GetHealth`
- `# SetHealth`
