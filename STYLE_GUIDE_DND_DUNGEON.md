# Style Guide — Card Game de D&D Dungeon Crawler

## 1) Direção de Arte (Visão Macro)

**Objetivo visual:** transmitir exploração perigosa, fantasia sombria e heroísmo desesperado em masmorras antigas.

**Pilares estéticos:**
- **Pintura digital escura** com acabamento semi-realista (textura de pincel visível, bordas levemente orgânicas).
- **Paleta predominante marrom / dourado / roxo** (terra, metal antigo e magia arcana).
- **Iluminação dramática vinda de baixo** (tochas, runas no chão, fogo alquímico, brilho infernal).
- **Perspectiva épica** (ângulo baixo, escala exagerada, sensação de grandeza e ameaça).

**Mood keywords:** `gothic-fantasy`, `dungeon-crawl`, `ancient-ruins`, `grim-heroic`, `arcane-corruption`.

---

## 2) Paleta de Cores (Base + Acentos)

### Cores base (70–80% da composição)
- **Marrom couro** — `#4A3426`
- **Marrom sombra** — `#2B1E18`
- **Dourado antigo** — `#B88A3B`
- **Bronze envelhecido** — `#8A6231`

### Cores secundárias (15–20%)
- **Roxo arcano** — `#5B3A78`
- **Roxo profundo** — `#3A254F`
- **Cinza pedra úmida** — `#5E5A57`

### Acentos de impacto (5–10%)
- **Dourado brilho** — `#E3C26A`
- **Magenta feitiço** — `#8F4FC9`
- **Vermelho perigo** — `#8F2A25` (usar com moderação para dano/sangue/risco)

**Regra de contraste:** elementos jogáveis (silhueta da criatura, arma, símbolo da carta) devem manter contraste claro/escuro de pelo menos 3 níveis de valor tonal em relação ao fundo.

---

## 3) Iluminação & Valor

**Assinatura obrigatória:**
- Fonte principal de luz **de baixo para cima** (underlighting).
- Luz secundária fria e fraca no topo para separar silhueta do fundo.
- Sombras densas em cavidades (olhos, frestas de armadura, cantos da dungeon).

**Efeitos recomendados:**
- Rim light dourada para personagens heróicos.
- Bloom roxo suave em magia (nunca estourar highlights).
- Partículas (cinzas, poeira, brasas) para profundidade atmosférica.

---

## 4) Composição & Perspectiva

- **Câmera baixa (contra-plongée)** para poder e escala.
- **Linhas de fuga convergindo** para o personagem/ameaça principal.
- Fundo com arcos, pilares quebrados, correntes, runas e névoa para narrativa ambiental.
- Evitar enquadramento “plano de retrato neutro”; toda carta deve contar uma micro-cena.

**Checklist de composição:**
1. Foco visual único em até 2 segundos.
2. Silhueta legível mesmo em miniatura.
3. Hierarquia: personagem > efeito principal > ambiente.

---

## 5) Direção por Tipo de Carta

### Monstro
- Postura agressiva, volume grande, sensação de peso.
- A luz de baixo deve deformar traços para parecer ameaçador.
- Roxo mais presente para corrupção, veneno, necromancia.

### Aliado/Herói
- Dourado mais dominante (esperança/ordem).
- Contraste entre desgaste (marrom) e nobreza (dourado).
- Expressão determinada, nunca “limpa demais”.

### Armadilha / Magia / Evento
- Composição focada no efeito em ação (chamas, runas, mecanismo ativando).
- Uso de cor de acento para leitura instantânea do efeito.

---

## 6) Moldura da Carta (UI/Frame)

- Moldura em **bronze/dourado envelhecido** com ornamentos discretos de runas.
- Janela de arte ocupando 60–65% da carta.
- Barra de título em marrom escuro com texto claro de alto contraste.
- Ícones de custo/ataque/vida com fundo escuro e borda dourada.
- Raridade representada por brilho no selo:
  - Comum: dourado opaco
  - Rara: dourado vivo + toque roxo
  - Lendária: dourado brilhante + aura roxa pulsante

---

## 7) Tipografia

- **Título:** serif medieval legível (ex.: Cinzel, Cormorant SC, ou equivalente).
- **Texto de regra:** sans serif limpa para leitura rápida (ex.: Inter, Noto Sans).
- Evitar mais de 2 famílias tipográficas.
- Títulos em `Small Caps` quando possível.

---

## 8) Prompt Base para Geração de Arte (IA)

Use este prompt como estrutura e personalize por carta:

> dark fantasy digital painting of [subject], inside an ancient dungeon, dramatic underlighting from below, epic low-angle perspective, dominant brown and antique gold with deep purple accents, painterly texture, high contrast chiaroscuro, volumetric fog, embers and dust particles, detailed armor/stone/metal, cinematic composition, trading card illustration, no modern elements, no sci-fi, no cartoon style

**Negative prompt sugerido:**

> bright daylight, flat lighting, pastel palette, modern city, sci-fi armor, anime style, low detail, washed colors, cute expression, comedic tone

---

## 9) 3 Cartas de Exemplo (Validação de Estilo)

## Carta Exemplo 1 — **Guardião da Cripta Quebrada**
- **Tipo:** Monstro — Morto-vivo
- **Custo:** 4
- **Ataque/Vida:** 5 / 4
- **Texto:** “Ao entrar, cause 1 de dano a todos os inimigos iluminados.”
- **Direção de arte:**
  - Cavaleiro esquelético gigante emergindo de um sarcófago partido.
  - Luz principal roxo-dourada vindo de runas no chão (de baixo).
  - Câmera baixa enfatizando altura e armadura antiga rachada.
  - Fundo com colunas destruídas e poeira suspensa.
- **Prompt da arte:**
  > undead skeletal knight rising from a shattered sarcophagus in a ruined crypt, dramatic underlighting from glowing purple-gold runes on the floor, epic low-angle perspective, dominant brown stone and antique gold armor, deep purple magical haze, dark fantasy digital painting, painterly details, cinematic shadows, trading card art

## Carta Exemplo 2 — **Clériga da Chama Oca**
- **Tipo:** Aliado — Humano
- **Custo:** 3
- **Ataque/Vida:** 2 / 5
- **Texto:** “No fim do turno, cure 2 de um aliado. Se ele estiver ferido, receba +1 de ataque até o próximo turno.”
- **Direção de arte:**
  - Clériga com incensário e maça ritual, manto gasto com detalhes dourados.
  - Chama sagrada subindo de um brasier no chão, iluminando o rosto de baixo.
  - Roxo discreto em símbolos arcanos no piso.
  - Perspectiva heróica, expressão firme e cansada.
- **Prompt da arte:**
  > battle-worn human cleric with ritual mace and censer, sacred flame rising from a floor brazier casting dramatic underlight, epic low-angle heroic pose, brown worn fabrics with antique gold trims, subtle purple arcane symbols on dungeon floor, dark fantasy digital painting, high contrast, textured brushwork, trading card illustration

## Carta Exemplo 3 — **Selo da Fenda Abissal**
- **Tipo:** Magia — Armadilha Arcana
- **Custo:** 2
- **Texto:** “Quando um inimigo atacar, reduza o ataque dele em 3 neste combate.”
- **Direção de arte:**
  - Runa circular rachando o piso da dungeon com energia roxa intensa.
  - Correntes espectrais surgindo da fenda e prendendo um monstro em salto.
  - Luz principal violeta de baixo com reflexo dourado em detritos.
  - Composição dinâmica em diagonal para sensação de ativação instantânea.
- **Prompt da arte:**
  > cracked abyssal rune seal erupting from dungeon floor, spectral chains bursting upward and binding a leaping monster, intense violet underlighting with antique gold reflections on debris, dramatic diagonal composition, dark fantasy digital painting, brown stone environment, epic perspective, trading card spell art

---

## 10) Critérios de Aprovação (QA Visual)

Uma carta está aprovada quando:
1. A paleta marrom/dourado/roxo é claramente dominante.
2. A fonte principal de luz vem de baixo e define a cena.
3. A leitura da silhueta funciona em tamanho reduzido.
4. A composição transmite escala e drama (perspectiva épica).
5. O acabamento parece pintura digital escura, não arte flat/cartoon.

