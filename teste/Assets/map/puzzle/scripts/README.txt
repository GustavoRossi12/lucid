=== Pressure Plate System ===

Sistema de placas de pressão que ativam um evento
quando um número mínimo delas é pisado.

=== Arquivos ===

- PressurePlate.cs       →  placa individual
- PressurePlateManager.cs →  gerencia um conjunto de placas

=== Como usar ===

1. CRIAÇÃO DAS PLACAS:
   - Para cada placa, crie um GameObject com:
     a) Collider (trigger)
     b) PressurePlate.cs
   - O Player deve ter a tag "Player"

2. CRIAÇÃO DO MANAGER:
   - Crie um GameObject vazio (ex: "PuzzleManager")
   - Adicione PressurePlateManager
   - No campo "Plates", defina a quantidade e arraste as placas
   - Em "Required Count", defina quantas precisam ser pisadas

3. EVENTO:
   - Em "On All Pressed", clique em +
   - Arraste o GameObject alvo e selecione a função desejada

=== Importante ===

- Agora você pode ter VÁRIOS managers na mesma cena,
  cada um com suas próprias placas
- A placa não depende mais de hierarquia (pode estar
  em qualquer lugar da cena)
