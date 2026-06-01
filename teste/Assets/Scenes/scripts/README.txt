=== SceneRandomizer ===

Sistema que gerencia transição entre salas aleatórias
e cenas de história no Unity, com contador de visitas.

=== Pastas de Cenas ===

- Scenes/salas aleatorias/   →  cenas de exploração (sorteio aleatório)
- Scenes/historia/           →  cenas de história (gatilhadas por contador)

=== Como usar ===

1. ANEXE o script a um GameObject vazio (ex: GameManager).

   ⚠ O script usa DontDestroyOnLoad automaticamente para que
     o contador persista entre as cenas.

2. CRIE um Text (TextMeshPro) na cena inicial para exibir o
   contador (ex: "Salas visitadas: 0").

3. ARRASTE esse TMP_Text para o campo "Counter Text" no Inspector.

4. No campo "Random Scenes", digite os nomes EXATOS das cenas
   da pasta "salas aleatorias".

5. No campo "Story Milestones", configure:
   - Visit Count: após quantas visitas essa cena deve aparecer
   - Scene Name: nome exato da cena na pasta "historia"

   Exemplo:
     Visit Count = 3, Scene Name = "Revelacao"
     → na 3a visita, carrega "Revelacao" em vez de uma aleatória

6. No Build Settings (File > Build Settings > Add Open Scenes),
   adicione TODAS as cenas (aleatórias e de história).

7. Para carregar a próxima cena, chame:

       FindObjectOfType<SceneRandomizer>().LoadRandomScene()

=== Métodos Públicos ===

- LoadRandomScene()  →  incrementa o contador, atualiza a UI,
                        verifica marcos de história e carrega
                        a cena adequada

=== Observações ===

- O contador "Total Visits" reinicia quando o jogo fecha.
  Para salvar entre sessões, implemente um SaveSystem.
- Se nenhum milestone bater, uma cena aleatória é carregada.
- As cenas DEVEM estar no Build Settings.
- O GameObject com este script NÃO deve ser destruído ao
  carregar cenas (DontDestroyOnLoad já está implementado).
