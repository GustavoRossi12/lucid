# Sistema de Batalha por Turnos — Estilo Honkai Star Rail
## Unity 2022.3+ | Batalha 2D com Ordem de Turnos (Mecânicas HSR)

---

## 📁 Estrutura do Projeto

```
TurnBattleSystem/
├── Scripts/
│   ├── Data/
│   │   ├── CharacterData.cs       — ScriptableObject: atributos, habilidades, elemento
│   │   ├── SkillData.cs           — ScriptableObject: dano, alvos, efeitos
│   │   └── StatusEffectData.cs    — ScriptableObject: DoTs, buffs, debuffs
│   ├── Characters/
│   │   └── BattleCharacter.cs     — Personagem em tempo real: HP, MP, energia, efeitos
│   ├── Battle/
│   │   ├── BattleManager.cs       — Loop de turnos central, sistema de Action Value
│   │   ├── EnemyAI.cs             — IA inimiga: Agressivo/Defensivo/Equilibrado
│   │   └── BattleSceneSetup.cs    — Inicializador de cena: instancia personagens
│   ├── UI/
│   │   ├── BattleUIManager.cs     — Controlador principal da interface
│   │   ├── PartyHUDCard.cs        — Card de HP/energia do personagem (inferior)
│   │   ├── EnemyHUDCard.cs        — Barra de HP/resistência do inimigo (superior)
│   │   ├── TurnOrderIcon.cs       — Retrato na barra de ordem de turnos
│   │   └── DamageNumber.cs        — Números flutuantes de dano/cura
│   ├── Effects/
│   │   └── EffectsManager.cs      — Trepidação de câmera, hit stop, spawn de VFX
│   └── Managers/
│       └── BattleAudioManager.cs  — Gerenciamento de música e efeitos sonoros
└── Docs/
    └── README.md                  — Este arquivo
```

---

## ⚙️ Mecânicas HSR Implementadas

### 1. Ordem de Turnos por Action Value (AV)
Os personagens agem com base em `actionValue = 10.000 / velocidade`. Quem tem o menor AV age primeiro.
Após cada turno, o AV é incrementado pelo AV base do personagem. Isso cria a ordem de turnos característica do HSR.

### 2. Pontos de Habilidade (SP)
- O grupo começa com **3 SP**, máximo **5 SP**
- **Ataque Básico** gera +1 SP
- **Habilidade** custa 1 SP (configurável por SkillData)
- **Ultimate** exige **Energia** cheia (100), não SP

### 3. Sistema de Energia
- Personagens ganham energia ao sofrer dano (+10) ou ao usar habilidades (configurável por SkillData)
- Ultimate só pode ser usada com 100/100 de energia
- A energia zera ao usar a Ultimate

### 4. Barra de Resistência (Weakness Break)
- Cada inimigo possui uma barra de Resistência exibida no EnemyHUDCard
- Ataques reduzem a Resistência gradualmente
- Quando a Resistência chega a 0: **Weakness Break** — o inimigo fica brevemente atordoado e recebe dano bônus
- A Resistência se reabastece após o período de Break

### 5. Efeitos de Status
- **DoT**: Queimadura/Sangramento/Choque causam `ataque * dotMultiplier` no início de cada turno
- **Controle**: Congelado/Aprisionado fazem o alvo perder o turno
- **Modificadores de Atributo**: Buffs e debuffs de ATQ/DEF/VEL
- **Escudos**: Absorvem dano antes de reduzir o HP
- **Regeneração**: Cura 5% do HP máximo por turno

---

## 🚀 Configuração Rápida

### Passo 1 — Importação e Dependências
1. Crie um **novo projeto Unity 2022.3+** (2D ou 3D, ambos funcionam)
2. Importe o **TextMeshPro** (Window → Package Manager → pesquise TextMeshPro)
3. Importe todos os scripts deste pacote para `Assets/Scripts/`

### Passo 2 — Criar ScriptableObjects

#### Character Data (Dados do Personagem)
`Assets → Create → TurnBattle → Character Data`

Preencha:
- `characterName`, `battleSprite`, `portrait`
- `element` (Fogo/Gelo/Raio/etc.)
- Atributos base: `baseHP`, `baseAttack`, `baseDefense`, `baseSpeed`
- Atribua os slots de habilidade: `basicAttack`, `skill`, `ultimate`

#### Skill Data (Dados da Habilidade)
`Assets → Create → TurnBattle → Skill Data`

Configurações principais:
- `skillType`: AtaqueBasico / Habilidade / Ultimate / Talento
- `targetType`: InimigoUnico / TodosInimigos / AlicoUnico / etc.
- `damageMultiplier`: ex.: 1.0 = 100% ATQ, 2.5 = 250% ATQ
- `skillPointCost`: 0 para Básico, 1 para Habilidade, 0 para Ultimate
- `energyGain`: energia que o usuário ganha ao usar (20 é o padrão)
- `hitCount`: habilidades multi-golpe (ex.: 3 para combo de 3 golpes)

#### Status Effect (Efeito de Status)
`Assets → Create → TurnBattle → Status Effect`

### Passo 3 — Hierarquia da Cena

```
BattleScene/
├── [Managers]
│   ├── BattleManager          (adicione BattleManager.cs)
│   ├── BattleUIManager        (adicione BattleUIManager.cs)
│   ├── EffectsManager         (adicione EffectsManager.cs)
│   └── BattleAudioManager     (adicione BattleAudioManager.cs)
├── [Setup]
│   └── BattleSceneSetup       (adicione BattleSceneSetup.cs)
├── [PontosDeSpawn]
│   ├── PlayerSpawn_0          (posição: -3, 0, 0)
│   ├── PlayerSpawn_1          (posição: -3, -1, 0)
│   ├── PlayerSpawn_2          (posição: -3, -2, 0)
│   ├── PlayerSpawn_3          (posição: -3, -3, 0)
│   ├── EnemySpawn_0           (posição: 3, 0, 0)
│   └── EnemySpawn_1           (posição: 3, -1, 0)
├── [UI] (Canvas — Screen Space Overlay)
│   ├── BarraDeOrdemDeTurnos   (layout horizontal, centro superior)
│   ├── ContainerPontosHabilidade (layout horizontal, canto inferior esquerdo)
│   ├── BotoesDeAcao
│   │   ├── BtnAtaqueBasico
│   │   ├── BtnHabilidade
│   │   └── BtnUltimate
│   ├── ContainerHUDGrupo      (inferior, horizontal)
│   ├── ContainerHUDInimigos   (superior esquerdo, vertical)
│   ├── PainelIndicadorDeTurno
│   ├── PainelVitoria
│   └── PainelDerrota
└── Camera Principal
```

### Passo 4 — Configurar BattleSceneSetup

No Inspector:
- Atribua `partyData[]` com seus assets CharacterData de jogadores
- Atribua `enemyData[]` com assets CharacterData de inimigos
- Atribua `partySpawnPoints[]` e `enemySpawnPoints[]`
- Crie um `battleCharacterPrefab` (veja abaixo) ou deixe nulo para geração automática

### Passo 5 — Prefab do Personagem

Crie um prefab com:
```
PrefabPersonagem/
├── SpriteRenderer     (componente de sprite)
├── Animator           (controlador com triggers: Attack, Skill, Ultimate, Hit, Die)
├── BattleCharacter    (script)
└── PontoVFX           (Transform filho vazio, no centro do sprite)
```

Em BattleCharacter, conecte:
- `spriteRenderer` → SpriteRenderer
- `animator` → Animator
- `vfxSpawnPoint` → PontoVFX

---

## 🎮 Fluxo de Jogo

```
InicioDaBatalha
    ↓
Calcula Action Values de todos os personagens
    ↓
[Loop]
    Encontra o personagem com menor AV → seu turno começa
    ↓
    Se JOGADOR:
        Exibe botões de ação → Aguarda input do jogador
        Jogador clica em AtaqueBasico/Habilidade/Ultimate
        (Para habilidades com alvo → modo de seleção de alvo)
    Se INIMIGO:
        EnemyAI.DecideAndExecute() → escolhe habilidade e alvo automaticamente
    ↓
    Executa habilidade (animação + dano + efeitos)
    ↓
    Avança AV, atualiza prévia da ordem de turnos
    ↓
    Verifica vitória/derrota → repete ou encerra
```

---

## 🔧 Expandindo o Sistema

### Adicionando um Novo Efeito de Habilidade
1. Crie um novo asset `StatusEffectData`
2. Em `SkillData.appliedEffects[]`, adicione o efeito
3. O efeito será aplicado automaticamente em `BattleCharacter.ApplyStatusEffect()`

### Adicionando um Novo Comportamento de IA
Em `EnemyAI.cs`, adicione um novo caso ao enum `AIBehavior` e atualize `ChooseSkill()`.

### Adicionando Troca de Personagem no Grupo
- Adicione uma interface com abas para cada membro que chama `BattleManager.Instance.activeCharacter = membroSelecionado`
- Conecte os botões de habilidade às habilidades do personagem recém-selecionado

### Adicionando Relíquias e Equipamentos
Crie um `RelicData : ScriptableObject` com multiplicadores de atributos.
Em `BattleCharacter.InitializeFromData()`, aplique os bônus das relíquias após o cálculo dos atributos base.

---

## 📋 Referência de Scripts

| Script | Função | Adicionar Em |
|--------|--------|--------------|
| `BattleManager` | Loop de turnos, SP, estado da batalha | GameObject Manager |
| `BattleCharacter` | HP/Energia/Efeitos em tempo real | Cada prefab de personagem |
| `EnemyAI` | Tomada de decisão do inimigo | Prefabs de inimigos |
| `BattleSceneSetup` | Spawn e conexão dos sistemas | GameObject Setup |
| `BattleUIManager` | Controle completo da interface | GameObject UI Manager |
| `PartyHUDCard` | Card de UI do membro do grupo | Prefab de card do grupo |
| `EnemyHUDCard` | Interface de HP/resistência do inimigo | Prefab de card de inimigo |
| `TurnOrderIcon` | Retrato na ordem de turnos | Prefab de ícone de turno |
| `DamageNumber` | Números flutuantes de dano/cura | Prefab de número de dano |
| `EffectsManager` | Trepidação de câmera, VFX | GameObject Manager |
| `BattleAudioManager` | Música e efeitos sonoros | GameObject Manager |

---

## 💡 Dicas

- **Velocidade é crucial**: Um personagem com 120 de velocidade age antes de um com 100 e age cerca de 20% mais vezes ao longo da batalha
- **Economia de SP**: Nunca desperdice Habilidades à toa; guarde 1 a 2 SP para curandeiros e suportes
- **Timing da Ultimate**: Ultimates ignoram o custo na ordem de turnos — são ações inseridas gratuitamente
- **Weakness Break**: Ataque as fraquezas dos inimigos para drenar a Resistência mais rápido e provocar o atordoamento do Break

---

*Desenvolvido para Unity 2022.3 LTS+ | Compatível com URP e Built-in Render Pipeline*
