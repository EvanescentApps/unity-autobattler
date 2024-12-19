# TheProject Unity

# Classes communes

### Entity
- `String name`
- `int price`
- `# GetName`
- `# GetPrice`

### Item
- `Entity entity`
- `List<ChampionAbility, float> upgrades`
- `# runUpgrades`

### Abstract ChampionAbility
- `# Upgrade (int)`

### Champion
- `Entity entity`
- `Health health`
- `Attack attack`
- `Mouvement mouvement`

### Mouvement
- `ChampionAbility championAbility`
- `# Upgrade`
- `float speed`
- `List<float> speedMultipliers`
- `# GetSpeed`
- `# SetSpeedMultiplier`
- `# GetSpeedMultipliers`

### Attack
- `ChampionAbility championAbility`
- `# Upgrade`
- `float cooldown` (0.2–1.5)
- `float distance`
- `float damage`
- `# GetDamage`
- `# SetDamage`

### Health
- `ChampionAbility championAbility`
- `# Upgrade`
- `float health`
- `# TakeDamage`
- `# GetHealth`
- `# SetHealth`
