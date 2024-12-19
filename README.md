# TheProject Unity

## Fonctionnalités
- Ce jeu est inspiré de Totally Accurate Battle Simulator
- Les attacks ne sont déclenchés que si on est sûre de toucher l'ennemi

## Classes communes

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
- `List<Item> items`
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
- `float cooldown` (0.2s–1.5s)
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
