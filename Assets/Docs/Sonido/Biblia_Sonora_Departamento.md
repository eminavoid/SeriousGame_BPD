# Biblia sonora — El departamento

Proyecto: *Un Día Con Emociones — TLP*
Uso: diseñar, grabar e implementar el audio de esta región en Wwise sin haber abierto el juego.
Entrega de esta etapa: proyecto Wwise + informe. Unity queda para el final de la materia.
Valores de partida: se ajustan de oído. Si se cambia un número, se cambia también la frase del informe.

## Cómo leer esto

Las secciones 1 a 4 alcanzan para entender el lugar y el criterio. La sección 5 es el mapa técnico: no nombrar nada distinto. Las secciones 6 a 11 son las fichas de construcción. La sección 12 es el guión exacto del Soundcaster. La sección 13 es la lista de grabación.

Prioridades:

- **P0** — sin esto el TP no cumple la consigna.
- **P1** — el Soundcaster cuenta el día. Entra en la entrega si el P0 ya suena.
- **P2** — profundidad. Se suma solo con el P0 cerrado.
- **P3** — final de la materia. Se anota el nombre ahora y no se implementa en Unity todavía.

## 1. El juego en una página

*Un Día Con Emociones* es un serious game en primera persona. No diagnostica y no reemplaza terapia. Simula un día en casa de una persona cuyas emociones suben rápido, duran y se enganchan a señales chicas: un visto, un plan cancelado, un golpe en la puerta, un comentario breve.

El jugador camina con WASD, mira con el mouse, interactúa con E y tiene desde el principio tres herramientas, en las teclas 1, 2 y 3:

- **STOP.** Parar, dar un paso atrás, observar, seguir. Se usa cuando el impulso es actuar ya.
- **TIPP (versión corta).** Bajar el cuerpo: agua fría, soltar tensión, respirar más lento (inhalar 4, sostener 2, exhalar 6).
- **Chequear hechos + acción opuesta.** Separar lo que pasó de lo que se interpretó y, si la emoción no encaja con el hecho, hacer lo contrario del impulso.

Hay tres números, siempre visibles:

| Medidor | Rango | Arranque | Lectura |
| --- | --- | --- | --- |
| Afecto | −100 a 100 | 10 | Cómo está el ánimo |
| Impulsividad | 0 a 100 | 25 | Alta desde 65. Desregulación desde 70 |
| Claridad | 0 a 100 | 55 | Baja desde 35 |

La variable de este TP es la impulsividad. Afecto y claridad se reservan.

El día corre de 07:30 a 23:00 en el reloj del juego.

| Bloque | Horario | ID de Wwise |
| --- | --- | --- |
| Mañana | 07:30–12:00 | `Manana` |
| Mediodía | 12:00–15:00 | `Mediodia` |
| Tarde | 15:00–20:00 | `Tarde` |
| Noche | 20:00–23:00 | `Noche` |

Hay dos maneras de entrar a una escena. **Push:** el mundo interrumpe (vibra el teléfono, golpean). **Pull:** el jugador se acerca (espejo, cama) y, si no va, el juego igual termina llevándolo.

El arco, para mezclar con sentido:

- Mañana: “quién soy” en el espejo, y “¿me dejan?” en el celular.
- Mediodía: un café cancelado parte la relación en todo o nada, y un golpe en la puerta no dice quién es.
- Tarde: una ola de emoción sin causa externa, un comentario de trabajo, la pareja que nombra a un ex.
- Noche: se decide si el día fue solo lo malo o también algo que se cuidó.

## 2. El lugar

Un departamento pequeño, techo de 2,65 m, una sola planta. El jugador no sale. El spawn está en el dormitorio, en (2.6, 1.05, 3.8), mirando hacia el estar.

Norte es +Z. Oeste es X = 0, donde está la ventana.

```
Norte (z ≈ 11,6)
+------------------ baño ------------------+------------------+
| canilla (3.2, 8.4)   espejo (3.9, 8.4)   | fondo sin uso    |
| puerta del baño: hueco x 1.0–2.75, z 6.2 | no se sonoriza   |
+---------------- dormitorio --------------+                  |
| cama (1.6, 1.5)   celular (2.9, 0.5)     |                  |
| ventana en x ≈ 0, z ≈ 3.3                |                  |
| paso al estar: hueco z 2.0–4.25, x 5.2   |                  |
+------------------------------------------+------------------+
| estar                          cocina abierta               |
| sillón (7.2, 4.7)   notebook (9.2, 3.0)  heladera (12.8, 4.9)|
| puerta de calle (11.0, 0.1), pared sur   café (12.7, 3.6)   |
+--------------------------------------------------------------+
Sur (z = 0)                                              Este
```

| Zona | Límites aproximados | Suelo (`Surface`) | Sala (`Recinto`) | Qué hay |
| --- | --- | --- | --- | --- |
| Dormitorio | x 0–5.2, z 0–6.2 | `Madera` | `Dormitorio` | Cama, mesa de luz, celular, ventana al oeste |
| Baño | x 0–4.4, z 6.2–10.4 | `Ceramico` | `Bano` | Espejo, canilla. Única pieza cerrada y dura |
| Estar | x 5.2–11, z 0–6.5 | `Madera`, y `Alfombra` en el área del sillón (x 6.2–8.2, z 4.1–5.4) | `Estar` | Sillón, notebook, puerta de calle |
| Cocina | x 11–13.6, z 2.5–6.5, sin muro | `Porcelanato` | `Estar` | Heladera y cafetera. Mismo aire que el estar |

El fondo norte (x > 5, z > 6.5) no tiene objetos ni escenas. No se diseña un ambiente para ese hueco.

El mesh del piso, en la build actual, es un solo material marrón. Las cuatro superficies son zonas acústicas de diseño. El final las va a marcar con triggers. En este TP se cambian a mano en el Soundcaster.

Materiales que sí se ven y conviene respetar al grabar: paredes del dormitorio cálidas, paredes del baño gris-azul (azulejo), madera en mesa y puerta, metal en canilla y heladera, vidrio en la ventana y el espejo, lino en la cama, tela oscura en el sillón.

## 3. Criterio

Un estímulo chico y una mezcla que se estrecha cuando la impulsividad sube.

El celular, la puerta y el notebook suenan a lo que son. El tamaño de la emoción no se dobla con un golpe musical ni con una disonancia de “alerta psicológica”. Lo dobla el RTPC: entra el pulso, la calle pierde agudos, el cuarto queda cerca. Cuando el RTPC baja, la calle vuelve. Esa vuelta es parte del diseño, porque en el juego las herramientas sirven para bajar el cuerpo, no para ganar una escena.

Decisiones que se mantienen en todas las fichas:

1. Pasos de medias, persona en su casa, primera persona. Sin tacos.
2. Tomas secas, micro cerca. La sala la pone el aux. Un toma grabado en el baño ya trae el azulejo y anula el state.
3. Lluvia y calle detrás del vidrio. No llueve sobre la cámara.
4. Sin voces. Ni pareja, ni amigo, ni delivery, ni pensamiento en off. El juego ya dice esas frases en texto.
5. Sin música en el día. Tone, heladera, calle y pulso ocupan ese lugar.
6. Sin gritos, sin vidrio roto, sin sirena, sin latido de trailer. El impulso de romper algo, si se graba, es una taza contra la mesada, una vez.
7. El silencio después de los ticks del chat es un evento de la demo: hay que dejarlo ocurrir, no taparlo con otra capa.
8. El espejo no agrega un archivo. Agrega el baño.

El corazón, la respiración y la UI no llevan reverb de sala. Los pasos y el foley de objetos sí. La calle, la lluvia y el viento no: si la lluvia tomara el RoomVerb del baño, cruzar la puerta cambiaría el clima.

## 4. Mapa de la consigna

| Pedido | Implementación | Prioridad |
| --- | --- | --- |
| Pasos, varias superficies, sin repetición | `Play_Footstep`, switch `Surface`, Shuffle + randomizer | P0 |
| Bioma y paisaje | `Play_Amb_Departamento` | P0 |
| Random de ambiente | Containers continuos de perro, risa, cañería, tele, auto | P0 |
| Clima con transición y viento variable | State `Precipitacion`, RTPC `IntensidadViento` | P0 |
| Recinto con states y reverb | State `Recinto`, tres aux RoomVerb | P0 |
| Una variable de juego | RTPC `Impulsividad` sobre pulso y calle | P0 |
| Soundcaster | `SC_Demo_TP1` | P0 |
| Hechos del día (celular, puerta, notebook) | Foley P1 | P1 |
| Objetos ya presentes en la casa sin escena propia | Fichas P2 | P2 |
| Posicionamiento, oclusión, Unity | Sección 14 | P3 |

## 5. Arquitectura Wwise

IDs en ASCII, sin tildes. Comentario corto en la raíz del Actor-Mixer: “Departamento. TP1. La región jugable es interior; la calle entra por la ventana del dormitorio.”

### 5.1. Carpetas

Work units: `WU_Player`, `WU_Ambience`, `WU_Foley`, `WU_Debug`.

```
Actor-Mixer
  Player
    Footsteps
      FS_Madera
      FS_Ceramico
      FS_Porcelanato
      FS_Alfombra
    Body
      Heartbeat
      Breath
  Foley
    Phone
    Door
    Laptop
    Sink
    Coffee
    Fridge
    Bed
    Couch
    Skills
    Window
  Ambience
    Interior
    Exterior
    Weather
    Punctual
  Debug
    Clap

Master-Mixer
  Master Audio Bus
    Bus_Body
    Bus_Footsteps
    Bus_Foley
    Bus_Narrative
    Bus_Ambience
      Bus_Interior
      Bus_Exterior
      Bus_Weather
    Bus_UI
  Aux_Reverb_Dormitorio
  Aux_Reverb_Estar
  Aux_Reverb_Bano
```

`Originals` repite esa lógica: `SFX/Player/Footsteps/Madera/`, `SFX/Ambience/Exterior/`, etc.

### 5.2. Game syncs

**Switch `Surface`:** `Madera`, `Ceramico`, `Porcelanato`, `Alfombra`.

**State `Recinto`:** `Dormitorio`, `Estar`, `Bano`. Transición: 0,35 s.

**State `Precipitacion`:** `Seco`, `Lluvia`. Transición: 2,5 s.

**State `Bloque`:** `Manana`, `Mediodia`, `Tarde`, `Noche`. Transición: 3 s.

**RTPC `Impulsividad`:** 0–100. Valor inicial de la sesión: 25.

**RTPC `IntensidadViento`:** 0–100. Valor inicial: 15.

### 5.3. Buses y quién lleva sala

| Bus | Lleva | Send a aux | Filtro de impulsividad |
| --- | --- | --- | --- |
| `Bus_Body` | Pulso, respiración | No | No |
| `Bus_Footsteps` | Pasos | Sí, según `Recinto` | No |
| `Bus_Foley` | Canilla, sábanas, taza, café, palmada de prueba | Sí | No |
| `Bus_Narrative` | Celular, puerta, notebook | No | No |
| `Bus_Interior` | Tone, heladera | No | Low-pass suave |
| `Bus_Exterior` | Ciudad, pájaros, perro, risa, autos | No | Low-pass y volumen |
| `Bus_Weather` | Lluvia, viento, castañeo | No | Casi no |
| `Bus_UI` | Click futuro | No | No |

Sends de `Bus_Footsteps` y `Bus_Foley`, controlados por `Recinto`. Los otros dos aux de cada fila van a −96 dB.

| State | Aux abierto | Nivel de send |
| --- | --- | --- |
| `Dormitorio` | `Aux_Reverb_Dormitorio` | −12 dB |
| `Estar` | `Aux_Reverb_Estar` | −8 dB |
| `Bano` | `Aux_Reverb_Bano` | −2 dB |

RoomVerb, punto de partida:

| Aux | Decay | Pre-delay | Ajuste |
| --- | --- | --- | --- |
| Dormitorio | 0,40 s | 10 ms | Mucha atenuación de agudos, estéreo angosto, difusión media |
| Estar | 0,65 s | 16 ms | Atenuación media. Es la sala más grande y tiene muebles |
| Baño | 1,25 s | 7 ms | Poca atenuación de agudos, primeras reflexiones más altas, difusión alta, estéreo un poco más ancho |

Tiene que alcanzar una palmada para reconocer el baño. Si hace falta subir el send del baño hasta 0 dB, se sube. Si el dormitorio empasta el paso, se baja el send antes de acortar todavía más el decay.

### 5.4. Curvas de `Impulsividad`

Blend del pulso, voz de volumen:

| RTPC | Volumen |
| --- | --- |
| 0–50 | −96 dB |
| 55 | −96 dB |
| 70 | −16 dB |
| 100 | −8 dB |

Cruce del blend: en 55 suena el loop lento; en 100, el rápido. El lento queda cerca de 66 bpm. El rápido, cerca de 112 bpm.

`Bus_Exterior`:

| RTPC | Voice Low-pass (0 abierto, 100 cerrado) | Volumen |
| --- | --- | --- |
| 0 | 0 | 0 dB |
| 70 | 25 | −4 dB |
| 100 | 50 | −8 dB |

`Bus_Interior`, solo low-pass, más tímido: 0 en RTPC 0, 20 en RTPC 100. El volumen de la heladera no se toca. A la noche, con la calle ya baja, la heladera queda. Con la impulsividad alta, la heladera también se oscurece un poco, y el pulso no: el cuerpo gana.

`Bus_Weather` puede bajar 3 dB entre 70 y 100. La lluvia no desaparece: deja de ser nítida. El pulso sigue seco.

### 5.5. Viento

`AMB_Wind_Window` y `AMB_Window_Rattle`, volumen de voz:

| `IntensidadViento` | Volumen |
| --- | --- |
| 0 | −96 dB |
| 30 | −28 dB |
| 60 | −14 dB |
| 100 | −6 dB |

El castañeo es un Random Container continuo, Shuffle, silencio aleatorio de 4 a 12 s entre tomas. Con el volumen en −96, el intervalo sigue corriendo y no se oye. Al subir el slider, el próximo castañeo aparece solo. Eso alcanza para la demo. No hace falta atar el intervalo al RTPC.

### 5.6. Eventos

| Evento | Tipo | Bus | Prioridad |
| --- | --- | --- | --- |
| `Play_Amb_Departamento` | Varias acciones Play, loops infinitos | Amb | P0 |
| `Stop_Amb_Departamento` | Stop de cada target de ese evento | Amb | P0 |
| `Play_Footstep` | Switch → Random, un disparo | Footsteps | P0 |
| `Play_Body_Heartbeat` | Blend en loop | Body | P0 |
| `Stop_Body_Heartbeat` | Stop | Body | P0 |
| `Play_Test_Clap` | Random, un disparo | Foley | P0 |
| `Play_Phone_Buzz` | Loop | Narrative | P1 |
| `Stop_Phone_Buzz` | Stop | Narrative | P1 |
| `Play_Phone_Typing` | Random continuo | Narrative | P1 |
| `Stop_Phone_Typing` | Stop | Narrative | P1 |
| `Play_Phone_Notify` | Random, un disparo | Narrative | P1 |
| `Play_Door_Knock` | Secuencia en loop | Narrative | P1 |
| `Stop_Door_Knock` | Stop | Narrative | P1 |
| `Play_Laptop_Notify` | Random, un disparo | Narrative | P1 |
| `Play_Sink_Water` | Loop | Foley | P1 |
| `Stop_Sink_Water` | Stop con fade 0,3 s | Foley | P1 |
| `Play_Body_Breath` | Un ciclo | Body | P1 |
| `Play_Bed_Sheets` | Random, un disparo | Foley | P1 |
| `Play_Door_Open` | Un disparo | Foley | P2 |
| `Play_Door_Close` | Un disparo | Foley | P2 |
| `Play_Bag_Rustle` | Un disparo | Foley | P2 |
| `Play_Coffee` | Secuencia corta | Foley | P2 |
| `Play_Fridge_Open` | Un disparo | Foley | P2 |
| `Play_Fridge_Close` | Un disparo | Foley | P2 |
| `Play_Couch` | Un disparo | Foley | P2 |
| `Play_Skill_Stop` | Una exhalación | Body | P2 |
| `Play_Skill_Breath` | Inhala 4, silencio 2, exhala 6 | Body | P2 |
| `Play_Skill_Page` | Hoja + trazo de birome | Foley | P2 |
| `Play_Cup_Hit` | Un disparo | Foley | P2 |
| `Play_UI_Select` | Un disparo, bajo | UI | P2 |

`Play_Amb_Departamento` incluye, cada uno con su acción Play: tone, heladera, ciudad, lluvia, viento, castañeo, y los random continuos de paisaje. En el Soundcaster se lanza una vez y se deja. El pulso va aparte (`Play_Body_Heartbeat`) para poder mostrarlo en silencio al inicio y presente al subir el slider, sin reiniciar la calle.

## 6. Personaje — pasos (P0)

Evento `Play_Footstep`.

Quién camina: una persona en medias, en su casa, cámara en primera persona, velocidad de juego 3,2 m/s. El paso es apoyo y un poco de roce. No hay taco, ni bota, ni pie descalzo mojado salvo un squeak ocasional de media contra cerámico.

Contenedor: Switch `Surface`. Cada rama, Random en **Shuffle**. Mínimo cuatro archivos, mejor seis. Randomizer de pitch −45 a +45 cents. Randomizer de volumen −2,5 a 0 dB. Esos randomizers van además del Shuffle: el Shuffle evita el mismo toma seguido; el pitch evita que, cuando el ciclo vuelva, sea idéntico.

Un archivo = un paso. Fade de entrada corto, cerca de 10 ms, para no recortar el ataque. Fade de salida 30–40 ms. Mono. Seco.

| Surface | Dónde | Cómo se oye | Relación de nivel |
| --- | --- | --- | --- |
| `Madera` | Dormitorio y estar fuera de la alfombra | Parqué, medias, cuerpo medio, cola corta | Referencia, 0 dB |
| `Alfombra` | Zona del sillón | Casi solo roce, muy corto, opaco | −4 dB respecto de la madera |
| `Ceramico` | Baño | Más agudo, más presente. Uno o dos tomas pueden chirriar | +1 dB |
| `Porcelanato` | Cocina | Más duro y seco que el cerámico del baño, sin chirriar | +1,5 dB |

En el final, Unity posteá un evento por paso, cerca de cada 0,5 s mientras haya desplazamiento. No convertir esto en un loop de Wwise: el loop no se entera de cuándo el personaje se detiene.

## 7. Cuerpo (P0 el pulso, P1 la respiración)

### `Play_Body_Heartbeat`

Dos loops mono, secos, grabados cerca: un lub-dub suave sobre una almohada o un cojín, no un latido de monitor clínico.

- `Heart_Slow`: un ciclo estable, cerca de 66 bpm, loopeado sin click.
- `Heart_Fast`: el mismo gesto, cerca de 112 bpm.

Blend Container en loop, cruzado por `Impulsividad` como en la sección 5.4. Bus `Bus_Body`. Sin aux. Sin low-pass de la calle.

A 25 no se oye. A 90 se oye delante del ambiente y no tapa un golpe de puerta ni el buzz. Si cansa el oído en la pasada completa del guión, bajar el tramo de 100 a −12 dB. Tiene que poder escucharse el demo entero.

En el juego, el latido placeholder solo dispara cuando la impulsividad es alta, cuando el afecto está en un extremo, o cuando una escena manda una “ola”. Esta curva cubre el caso de la impulsividad, que es el que el TP pide mostrar. El afecto queda para el final.

### `Play_Body_Breath` (P1)

Una inhalación y una exhalación, archivos separados, a unos 30 cm, fuera del eje del aire para que no sople la cápsula. Secuencia simple: inhala, pausa breve, exhala. Bus `Bus_Body`. Sirve para oír al personaje cuando una ficha lo pide, y como material del skill de respiración.

## 8. Ambiente (P0)

Evento `Play_Amb_Departamento`. Todo en loop o en random continuo. Estéreo las camas, mono los one-shots puntuales. La ciudad se graba con la ventana cerrada, o se filtra una sola vez al importar (un low-pass fijo, suave, en el Actor-Mixer del archivo, no en el RTPC). Ese filtro fijo es el vidrio. El low-pass del RTPC es la impulsividad y va encima, en el bus.

Niveles de partida, con impulsividad en 25, `Seco`, viento en 15, `Manana`. Se calibran para que se cumpla esta frase: el tone apenas está, la ciudad se oye por debajo de un paso, la heladera por debajo de la ciudad de día, un golpe de puerta se entiende a la primera.

### 8.1. Interior — `Bus_Interior`

| Objeto | Archivo | Comportamiento |
| --- | --- | --- |
| Tone | `AMB_RoomTone_Loop` | Loop estéreo, 20–30 s, pieza vacía, sin heladera y sin calle. Muy bajo |
| Heladera | `AMB_FridgeHum_Loop` | Loop del motor continuo, 15–20 s, mono o estéreo cerrado. No sigue al `Bloque` ni a la lluvia |

One-shots de interior, Random continuo, Shuffle, mínimo tres tomas:

| Objeto | Silencio entre disparos | Notas |
| --- | --- | --- |
| Cañería | 18–45 s | Expansión corta, metal lejano. Presente en todos los bloques |
| Tele del vecino | 35–80 s | Muy baja, ininteligible, sin palabras claras. Si el toma tiene diálogo, no se usa |

### 8.2. Exterior — `Bus_Exterior`

| Objeto | Archivo | Comportamiento |
| --- | --- | --- |
| Ciudad | `AMB_City_Glass_Loop` | Loop de tráfico lejano, ya detrás de vidrio |

Volumen de la ciudad según `Bloque` (encima de las curvas de impulsividad, no en lugar de ellas):

| Bloque | Ciudad |
| --- | --- |
| `Manana` | −6 dB |
| `Mediodia` | 0 dB |
| `Tarde` | −3 dB |
| `Noche` | −12 dB |

One-shots exteriores, mismo método continuo:

| Objeto | Silencio | State que lo deja oír | Resto de states |
| --- | --- | --- | --- |
| Pájaros, 4 tomas lejanas | 12–30 s | `Manana` y `Seco` a nivel bajo | −96 dB |
| Perro, 4 tomas | 25–70 s | Todos, más bajo de noche (−6 dB extra) | — |
| Risa lejana, 3 tomas, sin frases | 40–90 s | `Manana`, `Mediodia`, `Tarde` | `Noche` a −96 dB |
| Auto suelto, 4 tomas | 20–50 s | Sobre todo `Noche`, donde el loop de ciudad ya está bajo | De día puede existir muy bajo |

El perro y la risa no son un chiste ni una amenaza. Están en el texto de la ventana del juego: afuera el día ya empezó, hay autos, alguien riendo, un perro. Salen poco.

### 8.3. Clima — `Bus_Weather`

| Objeto | `Seco` | `Lluvia` |
| --- | --- | --- |
| `AMB_Rain_Glass_Loop` | −96 dB | 0 dB |
| Ciudad | según bloque | 4 dB menos que en seco |
| Pájaros | según bloque | −96 dB |
| Viento y castañeo | según el RTPC, en los dos states | igual |

La lluvia es impacto sobre vidrio o sobre una superficie dura cerca de una ventana, loop de 20–30 s sin truenos en primer plano. Un trueno muy lejano, una sola vez en un random de 60–120 s y solo en `Lluvia`, es opcional. Si se oye como susto, se saca.

El cruce se prueba así: con la ciudad sonando en `Seco`, pasar a `Lluvia` y no tocar ningún otro control durante 4 segundos. Se tiene que oír el vidrio entrando y los pájaros yéndose. Volver a `Seco` también es parte de la prueba.

## 9. Recintos (P0)

No hay un evento “de espejo”. La ficha del espejo es esta.

`Play_Test_Clap`: tres palmadas secas, mismo lugar, Random Shuffle, bus `Bus_Foley`, así que toman el send del state. Sirven para la corrección y se dejan en Debug para que se vea que no son un objeto del depto.

Prueba de oído, sin mirar el nombre del state:

- `Bano`: la palmada abre agudos y deja una cola corta pero clara. Azulejo, pieza chica, techo bajo.
- `Dormitorio`: la misma palmada casi seca. La cama se come la cola.
- `Estar`: en el medio, un poco más de aire que el dormitorio, menos brillo que el baño.

Después repetir la prueba con `Play_Footstep` en `Ceramico` dentro de `Bano` y en `Madera` dentro de `Dormitorio`. El switch y el state son dos controles distintos: se puede pisar cerámico “parado en el baño” dentro del Soundcaster aunque en el juego esa combinación sea la verdadera. También se puede pisar madera con el state del baño, y se tiene que oír raro a propósito: eso demuestra que superficie y sala no están atadas en secreto. En la demo, usar la combinación verdadera.

Por qué el baño es el recinto del enunciado: es la única geometría cerrada y reflectante, y es donde ocurre la primera escena del día. El personaje se mira y no sabe bien cómo está. La sala no agrega un stinger; agrega exposición. El dormitorio, a la noche, hace lo contrario: absorbe.

La cocina no tiene aux propio. Comparte `Estar` porque no hay puerta. El cambio de suelo a `Porcelanato` alcanza para oír que se llegó a la heladera.

## 10. Fichas del día

Formato de cada ficha: qué vive el jugador, qué se oye, qué no agregar, objeto de Wwise, sala de la demo.

El medidor “al entrar” es el salto que el juego aplica cuando la escena empieza, encima de lo que el jugador ya traiga.

### 10.1. Espejo — 07:36 — P0 por el state, sin archivo nuevo

ID de juego: `manana_espejo`. Ancla: espejo del baño. El jugador se acerca; si no va, el juego lo lleva.

Se despierta. Por un momento no sabe cómo está ni quién es hoy. No hay otra persona. El pensamiento es: “El baño está ahí. El espejo también. ¿Quién mirás hoy?”

Al entrar: afecto −10, impulsividad +5, claridad −5. Sigue siendo un número bajo. El pulso no entra.

Se oye: el ambiente de la mañana y, al pasar el state a `Bano`, la sala dura. Opcional P2: un roce mínimo de ropa, una sola vez, en `Bus_Foley`.

No se oye: golpe musical, respiración agitada, zumbido de terror, voz interior.

Demo: `Recinto = Bano`, `Bloque = Manana`, `Impulsividad = 25`, pasos en `Ceramico`.

### 10.2. Mensaje — 08:12 — P1

ID: `manana_mensaje`. Ancla: celular en la mesa de luz del dormitorio. Push: vibra.

Le escribió a alguien importante. Visto. “Escribiendo”. Dejó de escribir. Pasaron veinte minutos. El texto del juego: “El celular vibra. El pecho responde antes que la cabeza.”

Al entrar: afecto −25, impulsividad +30, claridad −15. Es el salto más brusco de la mañana. Si el día venía en 25, queda cerca de 55, justo donde el pulso empieza a asomar. En la demo se puede dejar el RTPC en 25 para oír el buzz limpio, y después subirlo a 60 para oír la misma vibración con el cuerpo encima. Las dos escuchas importan: el archivo no cambia, la mezcla sí.

`Play_Phone_Buzz`: tres tomas de vibración real sobre madera, loop, con un hueco corto (~200 ms) para que no sea un motor continuo. Bajo, físico, de mesa de luz. Reemplaza al seno de 180 Hz que el juego usa hoy.

`Play_Phone_Typing`: tres ticks diminutos de parlante, Random continuo, hueco 80–140 ms. Se cortan con `Stop_Phone_Typing` en medio de la ráfaga. No hay campanita de cierre.

Después del stop, la demo deja tres segundos del departamento solo. Ese silencio es el estímulo. Taparlo con música o con un acento rompe la escena.

Sala de la demo: `Dormitorio`. Bus `Bus_Narrative`, sin aux.

En el final, `PhoneController.StartBuzzing` postea el buzz y `StopBuzzing` lo corta. El audio es 3D en Unity, cerca de la mesa (0,4–8 m). En el TP se oye en estéreo, siempre, porque no hay engine: el informe lo dice para que no parezca un error de mezcla.

### 10.3. Café cancelado — 12:15 — P1

ID: `mediodia_cafe`. Otra vez el celular. Push.

Un amigo escribe: hoy no puede. En la cabeza del personaje, esa persona pasa de ser la mejor a no haberle importado nunca.

Al entrar: afecto −30, impulsividad +25, claridad −20.

`Play_Phone_Notify`: tres tonos cortos, familia distinta del buzz y de los ticks. Más agudo, más breve, una sola vez. Es un mensaje que llegó, no una vibración que espera. No es un sonido de “logro”.

Mismo bus narrativo, misma sala `Dormitorio` en la demo (el teléfono del juego no se mueve de la mesa de luz). `Bloque = Mediodia`: la calle está en su punto más presente, y el tono igual se tiene que entender.

No reutilizar `Play_Phone_Buzz`. Si suenan igual, se pierde la diferencia entre “me están dejando en visto” y “llegó un mensaje”.

### 10.4. Puerta — 13:06 — P1

ID: `mediodia_puerta`. Ancla: puerta de calle, pared sur del estar. Push. Es un beat extra: no avanza el día, enseña que cualquier señal ambigua se oye fuerte después del café.

Alguien golpea. El juego no dice quién es. Si se abre, es el delivery del vecino, puerta equivocada. Si no, los golpes siguen y después paran.

Al entrar: afecto −12, impulsividad +15, claridad −8.

`Play_Door_Knock` loopea patrones, no un solo hit. Tres o cuatro secuencias en un Random Shuffle:

- dos golpes a 180 ms, pausa ~800 ms, un golpe, pausa 1,8 s;
- tres golpes parejos a 220 ms, pausa 2 s;
- uno, pausa 400 ms, dos más rápidos, pausa 2 s.

Madera, nudillos, grabados del otro lado de la puerta cerrada, con el micro en la habitación. La sala del pasillo queda en el toma. Por eso este evento no lleva el aux del departamento: una segunda reverb lo duplicaría. Intensidad media, sin acelerar y sin crecer. No es una puerta que va a ceder.

`Stop_Door_Knock` corta. En el juego, mirar la puerta o terminar la escena llama a `StopKnocking`.

Demo: `Recinto = Estar`, `Surface = Madera`, `Bloque = Mediodia`. Aunque el state del estar esté activo, el knock no tiene que lavarse con el RoomVerb. Se comprueba muteando el aux: el golpe casi no cambia.

P2, desenlace de abrir: `Play_Door_Open`, `Play_Bag_Rustle`, `Play_Door_Close`. Bisagra, una bolsa, cierre. Sin “perdón” hablado. El dato correcto de la escena es que era un delivery; el audio lo dice con objetos.

### 10.5. Urgencia — 16:06 — P0 por el RTPC, sin archivo nuevo

ID: `tarde_urgencia`. Ancla: el cuerpo. No hay objeto que abrir. Push inmediato.

Está solo. Sube ira, tristeza o vacío sin una causa clara, y el cuerpo pide hacer algo ya. Es el pico de impulsividad del día: al entrar, afecto −35, impulsividad +40, claridad −25.

Se oye lo que ya está construido: `Impulsividad` en 85–95, pulso rápido, calle opaca, cuarto todavía presente. `Bloque = Tarde`.

No se oye: grito, vidrio, subidón musical, respiración de persecución.

P2: `Play_Cup_Hit`, una taza apoyada de más, seco, en `Bus_Foley`, una sola vez. Es el impulso de “romper algo chico” si se elige actuarlo. Después vuelve el mismo departamento. No es el sonido de la escena; es una consecuencia opcional.

La otra opción del juego es esperar diez minutos. En audio, esperar es bajar el slider y oír la calle volver, más el skill de respiración si está grabado.

### 10.6. Feedback — 17:27 — P1

ID: `tarde_feedback`. Ancla: notebook sobre la mesa del estar. Push: una notificación.

Alguien escribe “esto podría mejorar”. Adentro se oye como “no sirvo”.

Al entrar: afecto −20, impulsividad +20, claridad −15.

`Play_Laptop_Notify`: tres variaciones, click breve o campanilla muy chica de sistema, más seca y más baja que `Play_Phone_Notify`. Es trabajo, no es el chat de la mañana. Bus narrativo, sin aux. Demo en `Estar`, `Bloque = Tarde`.

No hay teclado todavía. El evento es la notificación, no la respuesta.

### 10.7. Pareja — 19:42 — P2 el sillón, P0 la sala

ID: `tarde_pareja`. Ancla: sillón. Push: la pareja cuenta el día en la casa.

De forma casual, cuenta que habló con un ex. El hecho es chico. La historia que arma el personaje es grande: “me va a dejar” o “me está mintiendo”.

Al entrar: afecto −24, impulsividad +22, claridad −16.

No se graba diálogo. `Play_Couch`: peso sobre tela, un solo gesto, `Bus_Foley`, con el send del `Estar`. Si no hay tiempo de grabarlo, la escena en el Soundcaster es el estar de tarde con la impulsividad en 60: el pulso apenas, la calle todavía ahí. El informe puede decir que la voz quedó afuera a propósito.

Zona de pasos de esa ancla: `Alfombra`. Conviene mostrarla en la demo cerca de esta ficha, para oír que al acercarse al sillón el paso casi se apaga y el cuarto queda desnudo.

### 10.8. Noche — 21:12 — P1

ID: `noche_cierre`. Ancla: cama. Pull: acercarse; si no, el juego cierra igual.

La casa está quieta. Se repasa el día. Es la única escena que entra un poco mejor de lo que venía: afecto +5, impulsividad −10, claridad +10. Si se rumia, esa ventana se cierra.

Demo: `Bloque = Noche`, `Recinto = Dormitorio`, `Impulsividad` cerca de 20, `Seco`, viento bajo. La ciudad a −12 dB, heladera en primer plano, sin pájaros, sin risa. `Play_Bed_Sheets`: dos o tres tomas de sábana, cortas, `Bus_Foley`.

No se oye: cama de suspense, reloj protagonista, tema melancólico. La noche del juego puede ser pesada por lo que la persona repasa, y el cuarto, mientras tanto, es solo un cuarto que se apagó.

## 11. Objetos de la casa y habilidades

Estos objetos ya están en el departamento. Algunos no tienen escena propia. Se diseñan ahora para que el final no improvise nombres.

### Ventana — P2

Ancla `window`, pared oeste del dormitorio. Texto del juego: afuera el día ya empezó, hay autos, una risa, un perro; el mundo no espera.

No abre un mundo nuevo. `Play` no hace falta si el ambiente ya contiene la calle. Si se quiere marcar la interacción, un state o un RTPC futuro `MirarVentana` sube `Bus_Exterior` 3 dB mientras dura. El vidrio sigue. La impulsividad, si está alta, sigue filtrando la calle: mirar afuera no aclara el cuerpo. Eso se puede explicar en el final; en el TP alcanza con tener la calle siempre detrás del loop.

### Canilla — P1 el loop, porque TIPP la usa

Ancla `sink`. “Agua fría en las muñecas. El cuerpo puede bajar un cambio.”

`Play_Sink_Water`: loop de chorro medio, mono, seco, `Bus_Foley` (sí lleva el baño). `Stop_Sink_Water` con fade de 0,3 s para que la canilla no corte en seco. Demo con `Recinto = Bano`.

En el final, la habilidad TIPP y la interacción de la canilla pueden compartir este evento.

### Heladera — el hum es P0; abrir es P2

Ancla `fridge`. “Luz, un resto de comida, nada que arregle el día.”

El motor ya está en el ambiente y no se apaga al abrir, o baja solo 2 dB mientras la puerta está abierta. `Play_Fridge_Open`: burlete, un segundo de espacio más brillante, algún frasco corto si entra solo. `Play_Fridge_Close`: cierre de goma, seco. Foley con send del `Estar`. Superficie de quien se acerca: `Porcelanato`.

### Café — P2

Ancla `coffee`. “El café ayuda un poco. No arregla la marea, pero calienta las manos.”

`Play_Coffee`: secuencia corta. Si hay cafetera o pava, usarla: calentar, verter, taza sobre madera. Tres a cinco segundos, sin loop eterno. `Bus_Foley`, `Estar`, de mañana tiene más sentido y en el Soundcaster se puede disparar con `Bloque = Manana`. No es un jingle de confort. Es un objeto caliente.

### Cama y sillón

Cubiertos en las fichas 10.7 y 10.8.

### Habilidades — P2

Son overlays de interfaz, no lugares nuevos. El audio es el gesto del cuerpo, en `Bus_Body` o foley seco, sin fanfarria de “skill completada”.

| Herramienta | Evento | Qué es |
| --- | --- | --- |
| STOP | `Play_Skill_Stop` | Una exhalación, cerca, seca. La pausa se oye porque no entra otra cosa |
| TIPP | `Play_Sink_Water` + `Play_Skill_Breath` | El agua ya definida. La respiración es una secuencia: inhala ~4, silencio ~2, exhala ~6. Una vuelta alcanza para la demo |
| Chequear hechos | `Play_Skill_Page` | Una hoja y un trazo corto de birome. Seco, cerca, `Bus_Foley` |

`Play_UI_Select`: un click mínimo, unos 18 dB por debajo de un paso, para los botones del final. En el TP puede existir y no entrar en el guión. Si se oye como menú de arcade, está fuerte.

### Debrief y aviso de contenido — no son esta región

Al terminar el día hay otra escena, de cierre y de recursos de ayuda. No se sonoriza en este TP. Cuando se haga, sigue la misma escala: sin tema épico, sin alarma. El lugar para una pieza musical, si la cátedra la pide en el final, es ese cierre, no el departamento.

## 12. Guión de `SC_Demo_TP1`

Dejar visibles: `Surface`, `Recinto`, `Precipitacion`, `Bloque`, `Impulsividad`, `IntensidadViento`.

Orden de eventos en la sesión, de arriba abajo: ambiente, pulso, pasos, palmada, teléfono (buzz, typing, notify), puerta, notebook, canilla, respiración, sábanas. Lo P2, debajo, separado.

Pasada. No saltar el silencio del punto 7.

1. **Mañana en el dormitorio.** `Play_Amb_Departamento` y `Play_Body_Heartbeat`. `Manana`, `Seco`, viento 15, `Dormitorio`, impulsividad 25, `Madera`. Escuchar tone, heladera atrás, calle baja, algún pájaro. El pulso no está.
2. **Pasos.** Ocho `Play_Footstep` en madera. Seis en `Alfombra`. Seis en `Ceramico`. Seis en `Porcelanato`. No se repite el mismo toma inmediato. La alfombra casi desaparece. El porcelanato es el más seco.
3. **Salas.** `Bano` y tres `Play_Test_Clap`. `Dormitorio` y tres. `Estar` y tres. Decir en voz alta la sala antes de mirar el state, como prueba.
4. **Clima.** Volver a dejar que se oiga la ciudad. `Lluvia`. Esperar cuatro segundos sin tocar otra cosa. Subir `IntensidadViento` a 100. Bajarlo a 15. Volver a `Seco` y esperar el cruce.
5. **Reloj.** `Mediodia` (la calle más presente), `Tarde`, `Noche`. De noche: `Play_Bed_Sheets`. La heladera queda adelante. `Impulsividad` sigue en 25.
6. **Cuerpo.** Volver a `Tarde` y a `Estar`. Subir impulsividad a 60, después a 75, después a 95. El pulso entra y se adelanta; la calle se apaga; la lluvia o el tone no se convierten en el pulso. Bajar a 20 y esperar a que la calle vuelva.
7. **Celular.** `Dormitorio`, impulsividad 25. `Play_Phone_Buzz` durante tres segundos, stop. `Play_Phone_Typing` durante cuatro segundos, stop. Contar tres segundos de departamento vacío. Un solo `Play_Phone_Notify`.
8. **Puerta y trabajo.** `Estar`. `Play_Door_Knock` unos seis segundos, stop. Un `Play_Laptop_Notify`. Canilla cuatro segundos con `Bano`, stop. Si existe, una `Play_Body_Breath`.

Duración aproximada de la pasada: cuatro minutos. Si un punto no se entiende sin señalar la pantalla, se corrige el audio, no el guión.

## 13. Grabación y archivos

WAV PCM, 48 kHz, 24 bits. Mono: pasos, foley, pulso, respiración, one-shots. Estéreo: tone, ciudad, lluvia, viento. Pico cerca de −3 dBFS. Fades 30–40 ms. Sin MP3 en `Originals`. Sin reverb, sin exciter, sin compresión fuerte: Wwise necesita el toma, no una mezcla previa.

Nombre: `FS_Madera_01.wav`, `AMB_City_Glass_Loop.wav`, `FOL_Knock_A_01.wav`. Números con dos dígitos.

### P0 — grabar sí o sí

| Qué | Cómo | Cantidad |
| --- | --- | --- |
| Pasos en medias | Teléfono o grabador a 40 cm del suelo, pieza silenciosa, un paso por archivo. Madera, cerámico, porcelanato, alfombra | 6 por suelo |
| Palmada | Misma pieza seca, misma distancia | 3 |
| Tone | 30 s, de noche si se puede, heladera apagada, ventana cerrada | 1 loop |
| Heladera | 20 s del motor ya estabilizado, sin la puerta | 1 loop |
| Ciudad | 45–60 s desde adentro, ventana cerrada | 1 loop |
| Pulso | Puño suave en almohada, muy cerca del micro. Dos golpes por ciclo. Armar después un loop a ~66 bpm y otro a ~112 bpm | 2 loops |

El chirriado de la media en el baño se conserva en uno o dos archivos. Es variación.

### P1

| Qué | Cómo | Cantidad |
| --- | --- | --- |
| Vibración | Celular vibrando sobre una mesa de madera, micro a 10 cm | 3 |
| Ticks | El parlante del teléfono, volumen bajo, un tick por archivo. Sirven como “escribiendo” | 3 |
| Notify de mensaje | Tres tonos cortos, distintos del tick. Pueden diseñarse en un editor con un sine muy breve y una cola de 80 ms, si no se quiere usar el ringtone real | 3 |
| Notify de notebook | Otro timbre, más seco y más bajo | 3 |
| Golpes | Otra persona, del otro lado de una puerta cerrada, micro en la habitación. Los tres patrones de la ficha 10.4 | 3 patrones |
| Canilla | Chorro medio, 8 s loopeables | 1 |
| Sábanas | Dos gestos cortos | 2 o 3 |
| Respiración | Inhala y exhala separados, a 30 cm, fuera del soplido | 2 |

### P2 y librería

Café, burlete de heladera, sillón, bolsa, taza, hoja de papel: grabarlos en casa si sobra una hora.

Lluvia sobre vidrio, viento de marco, pájaros, perro y risa lejana pueden ser librería si no se consiguen en el día. Prioridad de búsqueda: lluvia en ventana, no lluvia en bosque; risa lejana sin palabras; perro lejano, un solo ladrido por archivo. Licencia CC0 o equivalente, y una fila por lote en el informe. Si se graba la propia calle, no hace falta reemplazarla por una librería más “prolija”.

Lo que no se importa: latidos de ECG, puertas de dungeon, footsteps de armadura, truenos de trailer, notificaciones de “quest complete”.

## 14. Puente al final (P3)

No implementar ahora. No renombrar después.

| Señal en el juego | Llamada futura |
| --- | --- |
| Movimiento a 3,2 m/s | `Play_Footstep` cada ~0,5 s mientras haya desplazamiento. `Surface` según la zona de la sección 2 |
| Cruzar un cuarto | `SetState` de `Recinto` |
| `AffectState.Impulsivity` | `SetRTPCValue("Impulsividad", valor)` |
| `WorldClock` | `SetState` de `Bloque` |
| `PhoneController.StartBuzzing` / `StopBuzzing` | `Play_Phone_Buzz` / `Stop_Phone_Buzz` |
| `DoorController.StartKnocking` / `StopKnocking` | `Play_Door_Knock` / `Stop_Door_Knock` |
| Placeholder `ProceduralAudio` | Se deja de usar. Buzz, knock y seno de 55 Hz salen |

Atenuación para cuando haya posicionamiento, coherente con los AudioSource actuales del juego:

| Objeto | Dónde está | min / max |
| --- | --- | --- |
| Celular | Mesa de luz (2.85, 0.62, 0.52) | 0,4 m / 8 m |
| Puerta | (11.0, 1.1, 0.12) | 0,8 m / 12 m |
| Heladera | (12.75, 0.9, 4.85) | 1 m / 10 m |
| Canilla | (3.25, 0.45, 8.35) | 0,4 m / 6 m |
| Notebook | (9.15, 0.82, 2.95) | 0,5 m / 8 m |

El listener va en la cámara. Los pasos y el pulso siguen al jugador, sin atenuación: primera persona. La oclusión (oír la puerta más seca desde el baño) se resuelve en la integración; en este TP el golpe se demuestra como se oye desde el estar, que es donde ocurre la escena.

Volúmenes de trigger para el final, en metros de mundo, un poco metidos hacia adentro de los muros para no disparar en el vano:

- Dormitorio: x 0.2–5.0, z 0.2–6.0
- Baño: x 0.2–4.2, z 6.4–10.2
- Estar (incluye cocina): x 5.4–13.4, z 0.2–6.3
- Alfombra: x 6.2–8.2, z 4.1–5.4, prioridad por encima del estar
- Porcelanato: x 11.2–13.4, z 2.6–6.3, prioridad por encima del estar

## 15. Listo para zip

Se puede zippear cuando la pasada de la sección 12 sale de una vez, los créditos del informe tienen una fila por cada archivo que no grabó el grupo, y las siete capturas del informe muestran el proyecto real, no un esquema de este documento.
