# Estado del proyecto — 12 de noviembre de 2026

*Un Día Con Emociones — TLP*

Documento de producción: qué hace cada evento en el juego de hoy, cómo está armado el departamento y qué assets faltan. El porqué clínico de cada escena está en `Catalogo_Eventos_Eleccion.txt`. El diseño de audio está en `Sonido/Biblia_Sonora_Departamento.md`. Este archivo no los reemplaza: dice qué está implementado y qué hay que producir.

## 1. Qué es el juego hoy

Serious game en primera persona. Un solo día, dentro de un departamento. No diagnostica y no reemplaza terapia. El jugador camina, mira, interactúa con objetos de la casa y elige cómo responder cuando una emoción sube.

Flujo: aviso de contenido (`Boot`) → departamento (`House`) → cierre con glosario y recursos (`Debrief`). La escena `Day` queda como fallback de UI, sin el 3D.

Controles: WASD, mouse, `E` para interactuar, `1` STOP, `2` TIPP, `3` Acción opuesta. Las tres herramientas están desde el principio. Cooldown libre de 50 s.

Medidores, siempre visibles:

| Medidor | Rango | Arranque |
| --- | --- | --- |
| Afecto | −100 a +100 | 10 |
| Impulsividad | 0 a 100 | 25 |
| Claridad | 0 a 100 | 55 |

Alta impulsividad desde 65. Desregulación desde 70, o si el afecto llega a ±70. Claridad baja desde 35.

El contenido que se juega sale de `ContentFactory.cs` (plan armado en memoria). Los ScriptableObjects de `Resources/Content/` son un snapshot viejo de seis eventos: no incluyen la pareja ni la puerta, y el juego no los carga.

## 2. Cómo entra un evento

El día lo dirige `DayDirector`. Hay ocho beats. Siete cuentan para el día. La puerta es un beat extra (`CountsForDay = false`): enseña hipervigilancia y no suma un hito de progresión.

El reloj arranca en 07:30 y salta a la hora de cada beat. Entre beats avanza a 2,2 minutos de juego por segundo real, y se pausa mientras hay un evento o un overlay abierto. De noche (desde las 20:00) el sol baja y se pone azul.

Cada beat espera unos segundos y después queda pendiente. Si el jugador no lo toma, cada ~4 s se aplica un delta de posponer (el estado empeora). Si pasa el tiempo máximo, el juego lo fuerza.

| Disparo | Qué hace el jugador | Si no va |
| --- | --- | --- |
| Pull | Se acerca al ancla (`E`) | El juego lo abre igual al vencer el plazo |
| Push | El mundo interrumpe (vibra, golpean, notificación, ola) | Igual: se puede diferir un rato y después se fuerza |

Los eventos de celular (`manana_mensaje`, `mediodia_cafe`) abren la UI de chat. El resto abre el panel de evento. La urgencia del cuerpo no espera: al quedar pendiente se dispara sola.

Elegir una opción aplica su delta. Si la opción ofrece herramienta y el jugador la completa, se suma el bonus de esa opción y el efecto propio de la skill:

- STOP: afecto 0 / impulsividad −20 / claridad +15
- TIPP: afecto +8 / impulsividad −28 / claridad +10
- Acción opuesta: afecto +5 / impulsividad −15 / claridad +20

Ninguna opción se rotula incorrecta. El feedback es la consecuencia.

## 3. El departamento

Se construye en runtime con cubos de color (`HouseBuilder`). No hay prefabs, meshes ni materiales en disco. Techo 2,65 m. El jugador no sale. Spawn en el dormitorio, (2.6, 1.05, 3.8), mirando al estar. Norte es +Z. La ventana está al oeste (X = 0).

```
Norte (z ≈ 11,6)
+------------------ baño ------------------+------------------+
| canilla (3.2, 8.4)   espejo (3.9, 8.4)   | fondo sin uso    |
| vano del baño: x 1.0–2.75, z 6.2         | no se diseña     |
+---------------- dormitorio --------------+                  |
| cama (1.6, 1.5)   celular (2.9, 0.5)     |                  |
| ventana x ≈ 0, z ≈ 3.3                   |                  |
| vano al estar: z 2.0–4.25, x 5.2         |                  |
+------------------------------------------+------------------+
| estar                          cocina abierta               |
| sillón (7.2, 4.7)   notebook (9.2, 3.0)  heladera (12.8, 4.9)|
| puerta de calle (11.0, 0.1), pared sur   café (12.7, 3.6)   |
+--------------------------------------------------------------+
Sur (z = 0)                                              Este
```

| Zona | Límites | Qué hay | Superficie de diseño |
| --- | --- | --- | --- |
| Dormitorio | x 0–5.2, z 0–6.2 | Cama, mesa de luz, celular, ventana | Madera |
| Baño | x 0–4.4, z 6.2–10.4 | Espejo, canilla. Única pieza más cerrada | Cerámico |
| Estar | x 5.2–11, z 0–6.5 | Sillón, notebook, puerta | Madera; alfombra en el sillón (x 6.2–8.2, z 4.1–5.4) |
| Cocina | x 11–13.6, z 2.5–6.5, sin muro | Heladera, cafetera. Mismo aire que el estar | Porcelanato |

El fondo norte (x > 5, z > 6.5) no tiene objetos ni escenas. El piso, hoy, es un solo material marrón. Las cuatro superficies son de diseño (audio y arte); todavía no hay triggers.

Colores del greybox, para respetar cuando haya texturas: paredes del dormitorio cálidas, paredes del estar verde-gris, baño gris-azul, madera en mesa y puerta, metal en canilla y heladera, vidrio en ventana y espejo, lino en la cama, tela oscura en el sillón.

Luz: un direccional (sol) y un point de relleno. El sol se atenúa y enfría entre las 20:00 y las 22:30.

Cuando la impulsividad sube, la cámara cierra el FOV, hay un shake leve, una viñeta roja y un latido generado por código. En la urgencia esa “ola” se fuerza un momento. No hay post-proceso de autor, ni partículas, ni clima visible.

### Objetos y qué hacen si no hay evento

| Ancla | Prompt | Si se usa fuera de su beat |
| --- | --- | --- |
| `mirror` | Mirarte en el espejo | Pensamiento de identidad |
| `phone` | Mirar el celular | “La pantalla está quieta.” Si vibra: “Atender el celular” |
| `door` | Mirar la puerta | “Nadie.” Si golpean: “Atender la puerta” |
| `laptop` | Abrir el notebook | “La tapa cerrada.” En su beat: “Abrir el mail / comentario” |
| `bed` | Acostarte / cerrar el día | “Todavía no.” |
| `couch` | Sentarte un rato | Pensamiento del sillón como pozo |
| `sink` | Agua fría (TIPP) | Ofrece usar TIPP libre (tecla 2) |
| `window` | Mirar por la ventana | El día afuera ya empezó |
| `fridge` | Abrir la heladera | Comida, nada que arregle el día |
| `coffee` | Hacer café | Calienta las manos; no baja la marea |
| `body` | (no hay objeto) | La urgencia es una ola en la cámara |

El celular, cuando vibra, se sacude y suena un beep procedural, con rumble de gamepad. La puerta, cuando golpean, se mueve apenas y suena un knock procedural. Esos dos son los únicos sonidos del mundo, más el latido.

## 4. Qué hace cada evento

Arco del día: mañana, quién soy y si me dejan. Mediodía, el otro se parte en todo o nada y un golpe no dice quién es. Tarde, la emoción ya no necesita al otro, un comentario parte el valor propio, y la pareja nombra a un ex. Noche, se decide si el día fue solo lo malo.

### 1. Mañana — El espejo · 07:36 · `manana_espejo`

Pull en el espejo del baño. Espera 12 s, se puede posponer 20 s. Pensamiento: “El baño está ahí. El espejo también. ¿Quién mirás hoy?”

Estímulo interno. No hay otra persona. El jugador se mira y elige si tapa el malestar o lo nombra. Es el tutorial emocional del día.

Temas: identidad, vacío. Skill recomendada: STOP.

Al entrar: afecto −10, impulsividad +5, claridad −5. Si se pospone: −3 / +4 / −3 cada ~4 s.

| Opción | Medidor | Herramienta | Qué queda |
| --- | --- | --- | --- |
| Forzar una sonrisa y salir (“tengo que estar bien”) | −5 / +10 / −8 | STOP | Sin tool: máscara que pesa. Con STOP: nombra el vacío y sale con más aire. Bonus +5 / −10 / +12 |
| Nombrar en voz baja: “hoy me siento difuso/a” | +5 / −5 / +10 | No | Baja la presión. No lo arregla; ya no está solo con la niebla |

### 2. Mañana — Tres puntitos… y silencio · 08:12 · `manana_mensaje`

Push en el celular. Espera 9 s, se puede posponer 24 s. El celu vibra. Pensamiento: “El celular vibra. El pecho responde antes que la cabeza.” Si se evita: “Sigue ahí. Evitarlo no apaga la alarma: la sube.”

Abre el chat con “Alguien importante”: el jugador escribió, el otro quedó en “visto · escribiendo…” y después en “visto”. El silencio se lee como abandono.

Temas: abandono, idealización. Skill recomendada: TIPP.

Al entrar: −25 / +30 / −15. Si se pospone: −4 / +6 / −4.

| Opción | Medidor | Herramienta | Qué queda |
| --- | --- | --- | --- |
| Mandar varios mensajes (“¿estás enojado/a conmigo?”) | −15 / +20 / −20 | TIPP | Sin tool: urgencia y después vergüenza; estaba en una reunión. Con TIPP: espera y no incendia el chat. Bonus +15 / −25 / +15 |
| Esperar y distraerse | +5 / −10 / +8 | No | Cuesta. El silencio no era abandono: era la vida del otro |

### 3. Mediodía — Café con un amigo · 12:15 · `mediodia_cafe`

Push, otra vez el celular. Espera 9 s, se puede posponer 22 s.

En el chat llega “hoy no puedo, perdón”. Un plan chico se vuelve “nunca le importé”.

Temas: blanco/negro, abandono, ira. Skill recomendada: Acción opuesta.

Al entrar: −30 / +25 / −20. Si se pospone: −5 / +5 / −5.

| Opción | Medidor | Herramienta | Qué queda |
| --- | --- | --- | --- |
| Cortar: “no hace falta que nos veamos más” | −20 / +15 / −25 | Acción opuesta | Sin tool: vacío y “¿exageré?”. Con skill: canceló un plan, no te borró; se puede reagendar. Bonus +20 / −20 / +25 |
| “Me duele, espero otro día” | +10 / −10 / +15 | No | Duele igual. El puente no se quema |

### 4. Mediodía — La puerta · 13:06 · `mediodia_puerta`

Push en la puerta. Espera 6 s, se puede posponer 16 s. No cuenta como hito del día. Pensamiento: “Golpean. ¿Quién es? ¿Por qué ahora?” Si se evita: “Los golpes siguen. El pecho también.”

Justo después del café cancelado. El juego no dice quién es. Si se abre, era el delivery del vecino.

Temas: abandono. Skill recomendada: STOP.

Al entrar: −12 / +15 / −8. Si se pospone: −3 / +5 / −3.

| Opción | Medidor | Herramienta | Qué queda |
| --- | --- | --- | --- |
| Abrir | +8 / −12 / +10 | No | Era una puerta equivocada. El cuerpo tarda en bajar |
| No abrir | −10 / +8 / −8 | STOP | Sin tool: queda la duda. Con STOP: se observa el miedo y se puede chequear después. Bonus +8 / −12 / +12 |

### 5. Tarde — La urgencia en el pecho · 16:06 · `tarde_urgencia`

Push en el cuerpo. No hay objeto. Al quedar pendiente se abre sola. Pensamiento: “Una ola sube sin una causa clara. El cuerpo pide alivio YA.”

No hay mensaje ni otra persona. Es el evento de tolerancia al malestar. El juego no ofrece autolesión ni sustancias: el impulso es gritar, romper algo chico o mandar un mensaje hiriente.

Temas: desregulación, vacío, ira. Skill recomendada: TIPP (STOP en la segunda opción).

Al entrar: −35 / +40 / −25. Es el pico de impulsividad del día. Posponer casi no aplica, porque se dispara al entrar.

| Opción | Medidor | Herramienta | Qué queda |
| --- | --- | --- | --- |
| Actuar el impulso | −10 / −5 / −30 | TIPP | Sin tool: un segundo de descarga y después culpa. Con TIPP: la ola sigue, pero no arrastra igual. Bonus +25 / −35 / +20 |
| Surfear 10 minutos sin decidir nada grande | +5 / −15 / +10 | STOP | Sin tool: duele y baja un poco. Con STOP: la emoción es clima, no una orden. Bonus +10 / −20 / +18 |

### 6. Tarde — Un comentario en el trabajo / estudio · 17:27 · `tarde_feedback`

Push en el notebook. Espera 7 s, se puede posponer 20 s. Pensamiento: “Una notificación en el notebook. 'esto podría mejorar'.” Si se evita: “El comentario no se borra porque no lo abras.” El prompt pasa a “Abrir el mail / comentario”.

“Esto podría mejorar” se traduce a “soy un desastre, no sirvo”.

Temas: blanco/negro, identidad. Skill recomendada: Acción opuesta.

Al entrar: −20 / +20 / −15. Si se pospone: −3 / +4 / −3.

| Opción | Medidor | Herramienta | Qué queda |
| --- | --- | --- | --- |
| Hundirse: “siempre fallo, mejor ni intento” | −25 / +5 / −20 | Acción opuesta | Sin tool: el feedback se vuelve sentencia. Con skill: pidieron un ajuste; se pregunta qué priorizar. Bonus +20 / −15 / +22 |
| Pedir un ejemplo concreto y respirar | +8 / −12 / +12 | No | Duele. El diálogo vuelve a lo concreto |

### 7. Tarde — Habló con un ex · 19:42 · `tarde_pareja`

Push en el sillón. Espera 8 s, se puede posponer 18 s. Pensamiento: “Tu pareja cuenta el día. Nombró a un ex. El pecho ya armó una historia.” Si se evita: “La frase sigue en la mesa. Irte al otro cuarto no apaga la historia.”

El hecho es chico y fue contado, no descubierto. No hay pelea previa ni mensaje oculto. Validado por Psicología: Acción opuesta como herramienta principal; TIPP si la ansiedad es física.

Temas: abandono, blanco/negro. Skill recomendada: Acción opuesta.

Al entrar: −24 / +22 / −16. Si se pospone: −4 / +5 / −4.

| Opción | Medidor | Herramienta | Qué queda |
| --- | --- | --- | --- |
| Reclamar: “¿me vas a dejar por esa persona?” | −18 / +16 / −22 | Acción opuesta | Sin tool: el comentario se vuelve pelea. Con skill: te lo contó; no se usa de prueba. Bonus +18 / −16 / +22 |
| Pedirle el celular | −12 / +28 / −18 | TIPP | Sin tool: un segundo de alivio y la confianza rota. Con TIPP: no hace falta el celular. Bonus +14 / −24 / +14 |
| Quedarse: “¿y qué te contó?” | +8 / −12 / +14 | No | Duele, y el comentario sigue siendo un comentario |

### 8. Noche — Revisar el día · 21:12 · `noche_cierre`

Pull en la cama. Espera 12 s, se puede posponer 18 s. Pensamiento: “La casa se apaga. El día pide cierre — o rumia.”

Debrief todavía dentro del día. Es el único evento que entra un poco mejor. Si se rumia, el día se reescribe peor de lo que fue. Al terminar, el juego pasa al cierre (límites del juego, glosario, recursos).

Temas: identidad, desregulación. Skill recomendada: STOP, en la opción de cuidado, no en la de rumiar.

Al entrar: +5 / −10 / +10. Si se pospone: −4 / +5 / −5.

| Opción | Medidor | Herramienta | Qué queda |
| --- | --- | --- | --- |
| Rumiar todo lo “malo” | −15 / +10 / −15 | No | La noche agranda los fantasmas |
| Anotar 1 cosa difícil y 1 cosa que se hizo bien | +15 / −15 / +20 | STOP | Sin tool: hay lugar para lo difícil y para lo que se cuidó. Con STOP: cierre con un poco de compasión. Bonus +10 / −10 / +15 |

## 5. Assets que hay hoy

En `Assets` no hay modelos, texturas, sprites, animaciones, prefabs ni archivos de audio. La carpeta `Art/` que nombra `HOW_TO_PLAY.md` no existe.

| Capa | Estado |
| --- | --- |
| Código de día, casa, UI, skills, seguridad | Implementado |
| Geometría | Cubos primitivos, materiales de color creados al jugar |
| UI | Texto y paneles de uGUI en runtime. Fuente `LegacyRuntime` / Arial de Unity. Sin íconos |
| Audio en Unity | Tres clips generados: beep del celular, knock de la puerta, seno de latido. Sin ambiente, sin pasos |
| Wwise | Especificado en la biblia. No está el paquete en el proyecto |
| Personaje | Solo cámara y `CharacterController` (alto 1,8 m, radio 0,28 m, ojos a 1,6 m). Sin cuerpo ni manos |
| NPCs | No hay. Amigo, pareja y delivery existen como texto |
| Clima y exterior | No se ven. La calle está prevista solo como audio detrás del vidrio |
| Contenido en `Resources/Content/` | Seis eventos viejos. La pareja y la puerta viven solo en código |

## 6. Assets necesarios

Prioridad de producción. P0 es lo que hace legible el lugar y los disparadores. P1 completa el día. P2 es profundidad. Lo que no está en estas listas no hace falta para este proyecto.

Escala: departamento real, chico, una planta. Sin estilizado de cartoon y sin look de hospital. Los objetos tienen que leerse a 1–2 m de distancia en primera persona.

### 6.1. Entorno — piezas que reemplazan el greybox

Una malla por fila alcanza. Escala en metros. El pivot puede quedar en el origen del cubo actual.

| Pieza | Dónde | Para qué | Prioridad |
| --- | --- | --- | --- |
| Muros, piso y techo del departamento | Planta de la sección 3, vanos de baño y estar ya medidos | Reemplazar los cubos. El fondo norte puede quedar muro ciego | P0 |
| Cama + sábanas | (1.55, 0.32, 1.45), ~2.05 × 0.64 × 1.75 | Ancla de la noche | P0 |
| Mesa de luz | (2.85, 0.28, 0.52) | Apoyo del celular | P0 |
| Celular | (2.85, 0.62, 0.52), muy chico; el collider ya está agrandado | Vibra en dos beats. Hace falta una silueta que se lea | P0 |
| Espejo de pared | (3.85, 1.55, 8.35), ~0.7 × 0.95 | Ancla de la mañana. Puede ser un plano con reflejo falso; no hace falta un render de espejo real | P0 |
| Bacha / canilla | (3.25, 0.45, 8.35) | TIPP libre y agua | P0 |
| Sillón | (7.15, 0.38, 4.7), ~1.9 × 0.76 × 0.85 | Ancla de la pareja | P0 |
| Mesa + notebook cerrado | mesa (9.15, 0.38, 2.95); laptop (9.15, 0.82, 2.95) | Ancla del feedback | P0 |
| Puerta de calle | (11.0, 1.1, 0.12), ~1.08 × 2.2 | Golpea. No necesita animación de abrir para el MVP: el delivery se resuelve en texto | P0 |
| Ventana al oeste | (0.12, 1.45, 3.3), ~1.3 × 1.05 | La calle se oye detrás. Un vidrio opaco o una vista fija alcanza | P1 |
| Heladera | (12.75, 0.9, 4.85), ~0.7 × 1.8 × 0.7 | Interacción ambiente | P1 |
| Cafetera o pava + mesada | (12.7, 1.05, 3.55) | Interacción ambiente | P1 |
| Alfombra bajo el sillón | x 6.2–8.2, z 4.1–5.4 | Marca la superficie `Alfombra` | P1 |

Dressing que ayuda a leer “casa” y no cambia la jugabilidad: almohada, una taza en la mesada (sirve el foley de “romper algo chico”), un cuaderno en la mesa (sirve el foley de chequear hechos). P2.

No producir: exterior jugable, segundo piso, muebles del fondo norte, modelo de amigo, de pareja ni de delivery, cuerpo completo del jugador. Manos en primera persona son opcionales (P2) para la canilla y el celular; el juego hoy no las muestra.

### 6.2. Materiales

Cuatro suelos, porque el audio los va a distinguir: madera (dormitorio y estar), cerámico (baño), porcelanato (cocina), alfombra (zona del sillón).

Paredes: dormitorio cálida, estar más fría, baño azulejo gris-azul. Madera para mesa y puerta. Metal gastado para canilla y heladera. Lino para la cama. Tela oscura para el sillón. Vidrio simple para ventana y espejo.

Una textura por superficie, tileable, sin normal map de hero. El look tiene que aguantar la luz del sol de mañana y el mismo sol frío y bajo de la noche.

### 6.3. Interfaz

Todo es texto sobre paneles. Para cerrar el look hacen falta:

| Pieza | Uso | Prioridad |
| --- | --- | --- |
| Fuente con tildes y ñ, legible a 18–28 px | Reemplaza Arial de Unity en medidores, chat y paneles | P0 |
| Marco de celular | El chat ya es un panel oscuro vertical. Un frame (notch, hora, burbujas) lo separa del panel de evento | P1 |
| Tres íconos de herramienta | STOP, TIPP, Acción opuesta, junto a las teclas 1/2/3 | P1 |
| Barras de afecto, impulsividad y claridad | Hoy son rectángulos de color. Mismos tres números, mejor lectura | P1 |
| Viñeta | Ya existe como imagen a pantalla completa teñida de rojo. Un sprite de borde suave reemplaza el plano | P2 |

No hace falta HUD de inventario, minimapa ni retrato.

### 6.4. Audio

Especificación completa, nombres de eventos y guión de Soundcaster: `Sonido/Biblia_Sonora_Departamento.md`. Resumen de lo que hay que grabar o conseguir. WAV PCM, 48 kHz, 24 bits. Mono en foley y pasos. Estéreo en tone, ciudad, lluvia y viento. Sin voces. Sin música durante el día.

P0 — sin esto el departamento no suena a lugar:

| Qué | Cantidad |
| --- | --- |
| Pasos en medias: madera, cerámico, porcelanato, alfombra | 6 por suelo |
| Palmada seca (prueba de sala) | 3 |
| Tone de interior, heladera apagada | 1 loop de ~30 s |
| Motor de heladera ya estabilizado | 1 loop de ~20 s |
| Ciudad desde adentro, ventana cerrada | 1 loop de 45–60 s |
| Pulso (dos golpes por ciclo, ~66 bpm y ~112 bpm) | 2 loops |

P1 — los hechos del día. Reemplazan el beep, el knock y el seno de `ProceduralAudio`:

| Qué | Cantidad | Lo dispara |
| --- | --- | --- |
| Vibración de celular sobre madera | 3 | `PhoneController` al vibrar |
| Ticks de “escribiendo” | 3 | Chat del mensaje |
| Notify de mensaje, distinto del tick | 3 | Fin del silencio del chat |
| Notify de notebook, más seco y más bajo | 3 | Feedback |
| Golpes del otro lado de una puerta cerrada | 3 patrones | `DoorController` |
| Canilla, chorro medio loopeable | 1 de ~8 s | TIPP / pileta |
| Sábanas, gestos cortos | 2 o 3 | Noche |
| Respiración, inhala y exhala separados | 2 | Cuerpo / TIPP |

P2 — objetos que ya están y habilidades, si sobra tiempo de grabación: café, burlete de heladera, peso en el sillón, bolsa de delivery, taza contra mesada (una vez), hoja y birome, click de UI muy bajo. Lluvia sobre vidrio, viento de marco, pájaros, un ladrido lejano y una risa lejana pueden ser librería CC0.

No importar: latido de ECG, puertas de dungeon, pasos de armadura, truenos de trailer, jingle de quest, voces de la pareja o del amigo. El texto ya dice esas frases.

Wwise no está en el proyecto. La integración (pasos al caminar a 3,2 m/s, states de recinto y bloque horario, RTPC de impulsividad, reemplazo de `ProceduralAudio`) es el puente final de la biblia, no un asset de esta etapa.

### 6.5. Lo que el debrief pide y no es el departamento

El cierre muestra lo que pasó, el glosario (TLP, desregulación, miedo al abandono, pensamiento blanco/negro) y recursos de crisis. Sigue siendo UI de texto. Si más adelante hay una pieza musical, va ahí, no en la casa.

## 7. Orden útil de producción

1. Muros, piso y los ocho anclajes P0 (cama, celu, espejo, canilla, sillón, notebook, puerta), con las cuatro superficies aunque sea en color plano distinto.
2. Fuente y, si entra en la misma pasada, marco de celular.
3. Audio P0 (pasos, tone, heladera, calle, pulso) y P1 de celular, puerta y notebook, que son los únicos sonidos que el código ya intenta emitir.
4. Ventana, heladera, café y alfombra.
5. Dressing, manos, foley P2, e integración Wwise cuando el departamento ya se recorre.

El greybox actual ya tiene posiciones, colliders y nombres de ancla. Los assets nuevos pueden sustituir cada cubo sin mover la lógica de `DayDirector`.
