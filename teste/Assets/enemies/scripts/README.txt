=== EnemyChaser ===

Inimigo com sprite que persegue o jogador.

=== Funcionamento ===

- O sprite sempre fica virado para a câmera (billboard)
- O inimigo começa inativo (não se mexe)
- Chame Activate() para ativar a perseguição

=== Configuração (Inspector) ===

- Speed: velocidade de perseguição
- Aggro Range: distância que ativa a perseguição
- Stop Distance: para antes de colar no player
- Sprite: arraste o SpriteRenderer

=== Ativação por evento ===

No PressurePlateManager (ou qualquer script):
1. Em On All Pressed(), clique em +
2. Arraste o GameObject do inimigo
3. Selecione EnemyChaser > Activate()

=== Requisitos ===

- Player com tag "Player"
- Câmera principal com tag "MainCamera"
