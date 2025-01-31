# TheProject Unity
![image](https://github.com/user-attachments/assets/5c3437fe-c519-4c18-a99b-fa527eea4b72)

## Comment jouer/How to play

Download the game_build.zip file, unzip and run the executable

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
