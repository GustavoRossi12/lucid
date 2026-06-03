=== Pressure Plate System ===

=== Upgrade System (Mesa de Melhorias) ===

=== Arquivos ===

- PressurePlate.cs           →  placa individual
- PressurePlateManager.cs    →  gerencia um conjunto de placas
- UpgradeTable.cs            →  mesa que abre a UI de upgrades
- UpgradeUI.cs               →  painel de upgrades
- player/scripts/PlayerStats.cs  →  pontos e níveis do jogador

=== Upgrade Table (Mesa) ===

1. Crie um GameObject com Collider trigger (Ex: modelo de mesa)
2. Adicione UpgradeTable.cs
3. Crie um Canvas com UpgradeUI.cs e atribua no campo "UI"
4. No UpgradeUI, atribua os textos (Points, Speed, Dash, Jump)
5. Crie 3 botões e conecte: BuySpeed, BuyDash, BuyJump

=== Player Stats ===

- Adicione PlayerStats.cs ao jogador
- Para dar pontos, chame de qualquer script:
    FindObjectOfType<PlayerStats>().AddPoints(1)

=== Como Ganhar Pontos ===

- No PressurePlateManager.OnAllPressed → PlayerStats.AddPoints
- No EnemyChaser (quando morrer) → PlayerStats.AddPoints
- No DialogueManager (fim do diálogo) → PlayerStats.AddPoints
