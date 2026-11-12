# Reparto en tres — TP1 Wwise

Cada integrante tiene su parte cerrada: qué grabar, qué armar en Wwise y qué escribir. Los nombres salen de `Biblia_Sonora_Departamento.md`. No se renombran.

Los menús de Wwise están en inglés. Este paso a paso usa esos nombres.

| | Nombre | Parte | Horas |
| --- | --- | --- | --- |
| A | | Pasos, salas y esqueleto del proyecto | ~4 h |
| B | | Ambiente, horario y clima | ~4 h |
| C | | Pulso, hechos del día, sesión y entrega | ~4 h |

Wwise lo abre una sola persona por vez. El archivo del proyecto pasa de A a B y de B a C. Grabar, editar WAVs y escribir el informe se hace en paralelo, mientras otro tiene el proyecto.

## Antes de separarse

1. Anoten el nombre en la tabla de arriba.
2. Lean la sección 3 de la biblia (criterio). Quince minutos, los tres.
3. Abran una carpeta compartida con tres subcarpetas: `A`, `B` y `C`. Ahí se dejan los WAV ya nombrados, antes de importar.
4. Combinen diez minutos para la puerta: C deja el micrófono dentro de una habitación, con la puerta cerrada. A o B golpea del otro lado. Los patrones están en el paso 4 de C.

Norma para los tres, en cada archivo: WAV PCM, 48 kHz, 24 bits, pico cerca de −3 dBFS, fade de salida de 30 a 40 ms, sin reverb y sin compresión. Foley y pasos en mono. Camas de ambiente en estéreo. Un gesto por archivo.

## Cadena

1. Los tres graban y editan a la vez.
2. A construye el proyecto vacío, después los pasos y la reverb. Avisa con el mensaje de entrega de A.
3. B importa el ambiente y el clima. Avisa con el mensaje de entrega de B.
4. C importa el cuerpo y el foley, ata la impulsividad y ordena `SC_Demo_TP1`.
5. Los tres escuchan el guión del paso 11 de C. Quien oye un problema lo anota. Lo corrige solo el dueño de esa capa.
6. Cada uno pega sus capturas y sus créditos. C exporta el PDF y arma el zip.

---

# A — Pasos y salas

En la corrección explicás las cuatro superficies y por qué el baño suena distinto del dormitorio.

Tu audio: pasos en medias y tres palmadas secas.
Tu Wwise: esqueleto, `Play_Footstep`, `Play_Test_Clap`, tres RoomVerb.
Tu informe: secciones 4, 5.1 y 5.4 de `Informe_TP1_Wwise.md`. Capturas 2, 3 y 5.

## Paso 1 — Grabar los suelos

Medias puestas. El teléfono o el grabador va a unos 40 cm del suelo, en la pieza más apagada de la casa, lejos del baño. Si el toma ya suena a azulejo, la reverb de Wwise no se va a notar.

Un paso por toma. Apoyo y un poco de roce. Sin taco y sin pisotón.

1. Seis pasos en parqué o madera: `FS_Madera_01` a `FS_Madera_06`.
2. Seis en el cerámico del baño: `FS_Ceramico_01` a `FS_Ceramico_06`. Si una media chirria, dejalos. Sirven uno o dos, no los seis.
3. Seis en un piso duro de cocina, distinto del baño: `FS_Porcelanato_01` a `FS_Porcelanato_06`. Más secos, sin chirrido.
4. Seis sobre alfombra: `FS_Alfombra_01` a `FS_Alfombra_06`. Casi solo roce.
5. Tres palmadas en la misma pieza seca, a la misma distancia: `DBG_Clap_01` a `DBG_Clap_03`.

## Paso 2 — Editar

En Audacity, o el editor que usen:

1. Recortá cada paso a un solo apoyo. Dejá el ataque. Fade de entrada de unos 10 ms, solo para que no haga click. Fade de salida de 30 a 40 ms.
2. Pasá cada archivo a mono, 48000 Hz, 24 bits.
3. Normalizá el pico a −3 dB.
4. Exportá con el nombre final a la carpeta `A`.
5. Escuchá la madera contra la alfombra. La alfombra tiene que ser más corta y más opaca. Si suenan igual, volvé a grabar la alfombra más cerca de la tela y más lejos del piso de abajo.

## Paso 3 — Crear el proyecto y las carpetas

1. En el Wwise Launcher, New Project. Nombre: `Departamento_TLP`. Ubicación compartida, para que B y C lo puedan abrir.
2. En el Project Explorer, sobre Actor-Mixer Hierarchy, creá cuatro Work Units: `WU_Player`, `WU_Ambience`, `WU_Foley`, `WU_Debug`.
3. Adentro, estas carpetas vacías (Virtual Folders):

```
WU_Player
  Footsteps
  Body
WU_Ambience
  Interior
  Exterior
  Weather
  Punctual
WU_Foley
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
WU_Debug
  Clap
```

4. En la raíz del Actor-Mixer Hierarchy, Notes: `Departamento. TP1. La calle entra por la ventana. La reverb va solo en pasos y foley.`

## Paso 4 — Buses

Sobre Master Audio Bus, New Child > Audio Bus, en este orden:

- `Bus_Body`
- `Bus_Footsteps`
- `Bus_Foley`
- `Bus_Narrative`
- `Bus_Ambience`
- `Bus_UI`

Sobre `Bus_Ambience`, tres hijos: `Bus_Interior`, `Bus_Exterior`, `Bus_Weather`.

Sobre Master Audio Bus, New Child > Auxiliary Bus:

- `Aux_Reverb_Dormitorio`
- `Aux_Reverb_Estar`
- `Aux_Reverb_Bano`

Los efectos van en el paso 8. Ahora los aux pueden quedar sin plugin.

## Paso 5 — States, switch y RTPCs

En Game Syncs > States, un State Group por fila. Adentro, New Child > State. IDs sin tildes.

| State Group | States | Transition time del grupo |
| --- | --- | --- |
| `Recinto` | `Dormitorio`, `Estar`, `Bano` | 0,35 s |
| `Precipitacion` | `Seco`, `Lluvia` | 2,5 s |
| `Bloque` | `Manana`, `Mediodia`, `Tarde`, `Noche` | 3 s |

La transición se edita en el State Group, pestaña Transitions. Default transition time con el valor de la tabla.

En Game Syncs > Switches, Switch Group `Surface` con `Madera`, `Ceramico`, `Porcelanato`, `Alfombra`.

En Game Syncs > Game Parameters:

1. `Impulsividad`. Rango 0 a 100. Initial 25. Sin curva todavía.
2. `IntensidadViento`. Rango 0 a 100. Initial 15. Sin curva todavía.

B y C atan las curvas. Si las dibujás vos, después no coinciden con lo que ellos importan.

## Paso 6 — Eventos vacíos

En Events, un Work Unit `WU_Events`. Creá cada evento de esta lista, todavía sin acción, salvo los dos que armás en los pasos 7 y 8.

`Play_Amb_Departamento`, `Stop_Amb_Departamento`, `Play_Footstep`, `Play_Body_Heartbeat`, `Stop_Body_Heartbeat`, `Play_Test_Clap`, `Play_Phone_Buzz`, `Stop_Phone_Buzz`, `Play_Phone_Typing`, `Stop_Phone_Typing`, `Play_Phone_Notify`, `Play_Door_Knock`, `Stop_Door_Knock`, `Play_Laptop_Notify`, `Play_Sink_Water`, `Stop_Sink_Water`, `Play_Body_Breath`, `Play_Bed_Sheets`.

Esos nombres quedan fijos. B y C agregan la acción Play o Stop al evento que ya existe. No crean otro al lado.

## Paso 7 — Pasos

1. Sobre `Footsteps`, New Child > Switch Container. Nombre: `FS_Switch`.
2. En sus propiedades, Switch Group = `Surface`. Output Bus = `Bus_Footsteps`.
3. Adentro, cuatro Random Containers: `FS_Madera`, `FS_Ceramico`, `FS_Porcelanato`, `FS_Alfombra`.
4. Asociá cada container al switch del mismo nombre. Arrastrá el container a la fila del switch en el editor de contenidos.
5. En cada Random Container: Play Type = Shuffle. Randomizer de pitch de −45 a +45 cents. Randomizer de volumen de −2,5 a 0 dB.
6. Importá los seis WAV de cada suelo dentro de su container. Wwise los copia a Originals. Si pide la carpeta, que coincida con el suelo.
7. Volumen del container, encima del randomizer: madera 0 dB, alfombra −4 dB, cerámico +1 dB, porcelanato +1,5 dB.
8. Abrí el evento `Play_Footstep`. Add Action > Play. Target: `FS_Switch`. Scope del evento: un disparo, sin loop. Cada clic del Soundcaster es un paso.
9. Probá ya, aunque la reverb todavía no esté. En una sesión temporal o en `SC_Demo_TP1`, dispará madera ocho veces. No tiene que volver el mismo archivo dos veces seguidas. Si vuelve, el container quedó en Sequence o en Standard, no en Shuffle.

## Paso 8 — Reverb

1. En cada aux, pestaña Effects, Add > RoomVerb. Edit y dejá este punto de partida:

| Aux | Decay time | Pre-delay |
| --- | --- | --- |
| `Aux_Reverb_Dormitorio` | 0,40 s | 10 ms |
| `Aux_Reverb_Estar` | 0,65 s | 16 ms |
| `Aux_Reverb_Bano` | 1,25 s | 7 ms |

2. En el dormitorio, atenuá agudos y angostá el estéreo. En el baño, dejá los agudos más vivos, subí las primeras reflexiones y abrí un poco el estéreo. El estar queda entre los dos.
3. En `Bus_Footsteps` y en `Bus_Foley`, activá User-Defined Auxiliary Sends y asigná los tres aux.
4. En cada send, clic derecho > Bind to State > `Recinto`. Valores:

| State | Dormitorio | Estar | Baño |
| --- | --- | --- | --- |
| `Dormitorio` | −12 dB | −96 dB | −96 dB |
| `Estar` | −96 dB | −8 dB | −96 dB |
| `Bano` | −96 dB | −96 dB | −2 dB |

5. `Bus_Body`, `Bus_Narrative`, `Bus_Ambience` y `Bus_UI` quedan sin sends. Si la lluvia o el pulso entran al baño, el state está mal ruteado.
6. Sobre `Clap`, un Random Container `DBG_Clap`, Shuffle, Output Bus `Bus_Foley`. Importá las tres palmadas.
7. Evento `Play_Test_Clap`: acción Play hacia `DBG_Clap`.

## Paso 9 — Sesión y prueba tuya

1. En Soundcaster Sessions, New > `SC_Demo_TP1`.
2. Arrastrá a la sesión, de arriba abajo: `Play_Footstep`, `Play_Test_Clap`. Dejá hueco arriba para el ambiente que va a meter B.
3. En el panel de game syncs de esa sesión, agregá `Surface`, `Recinto`, `Precipitacion`, `Bloque`, `Impulsividad`, `IntensidadViento`.
4. Valores iniciales de la sesión: `Madera`, `Dormitorio`, `Seco`, `Manana`, impulsividad 25, viento 15.
5. Ocho clics de `Play_Footstep` en cada superficie.
6. `Play_Test_Clap` tres veces en `Bano`, tres en `Dormitorio`, tres en `Estar`. Tapá el nombre del state y decí la sala antes de mirarlo. El baño se tiene que reconocer en el primer golpe.
7. Si el baño no se separa, subí su send hasta 0 dB antes de alargar más el decay. Si el dormitorio embarra el paso, bajá su send. No le pongas reverb al archivo.

## Paso 10 — Informe y captura

Cuando B te pide el proyecto, guardá y cerrá Wwise. Mientras B y después C trabajan:

1. Completá en el informe las secciones 4, 5.1 y 5.4 con los valores que quedaron de verdad. Si moviste un send, el texto tiene que decir ese número.
2. Captura 2: Master-Mixer con buses y aux a la vista.
3. Captura 3: un Random Container de pasos donde se lea Shuffle y el randomizer.
4. Captura 5: el RoomVerb del baño.
5. En la tabla de créditos, una fila: pasos y palmadas, grabación del grupo, tu nombre.

## Mensaje de entrega a B

`Proyecto listo para ambiente. Play_Footstep y Play_Test_Clap andan. States, switch y RTPCs creados, sin curvas. Eventos vacíos con el nombre final. No muevas sends ni Surface.`

## Si sobra tiempo, después de la prueba del paso 9

`Play_Cup_Hit`: una taza apoyada de más, un solo archivo, en `WU_Foley` o en Skills, bus `Bus_Foley`. `Play_Skill_Page`: una hoja y un trazo de birome, mismo bus. Los dos toman la sala solos. No los hagas si la palmada del baño todavía no se distingue.

## No abras

Carpetas `Interior`, `Exterior`, `Weather`, `Punctual` una vez que B importó. Curvas de `IntensidadViento` y de `Impulsividad`. Celular, puerta, pulso.

---

# B — Departamento y clima

En la corrección explicás el paisaje del depto, el cruce de seco a lluvia y el viento.

Tu audio: tone, heladera, calle, y la librería que falte.
Tu Wwise: `Play_Amb_Departamento`, `Bloque`, `Precipitacion`, `IntensidadViento`.
Tu informe: secciones 2, 5.2 y 5.3. Captura 4.

Mientras A arma el esqueleto, hacé los pasos 1 a 3. No abras el proyecto hasta el mensaje de entrega de A.

## Paso 1 — Grabar las tres camas

1. `AMB_RoomTone_Loop`. 30 segundos. Heladera apagada, ventana cerrada, nadie en la pieza. Estéreo.
2. `AMB_FridgeHum_Loop`. 15 a 20 segundos del motor ya estabilizado, sin abrir la puerta. El arranque del motor no entra al loop.
3. `AMB_City_Glass_Loop`. 45 a 60 segundos desde adentro, ventana cerrada. Si la calle de tu casa está vacía, sirve igual: es el depto, no un mapa de ciudad.

Buscá un punto de loop. El final y el principio tienen que parecerse. Un crossfade corto en el editor, de 200 a 400 ms, evita el click. No le pongas reverb.

## Paso 2 — Conseguir el resto

Freesound u otra librería, filtro CC0 o equivalente. Descartá el archivo si es lluvia de bosque, trueno de trailer, perro en primer plano o una risa con palabras.

| Archivo | Cantidad | Qué buscar |
| --- | --- | --- |
| `AMB_Rain_Glass_Loop` | 1 loop de 20–30 s | Lluvia contra vidrio o contra una superficie dura |
| `AMB_Wind_Window_Loop` | 1 loop | Aire en un marco, sin soplido a la cápsula |
| `AMB_Birds_01` … `04` | 4 | Pájaros lejanos, cada uno un gesto |
| `AMB_Dog_01` … `04` | 4 | Un ladrido lejano por archivo |
| `AMB_Laugh_01` … `03` | 3 | Risa lejana, sin frases |
| `AMB_Car_01` … `04` | 4 | Un auto que pasa, lejos |
| `AMB_Pipe_01` … `04` | 3 o 4 | Cañería o click metálico corto |
| `AMB_TV_01` … `03` | 3 | Tele detrás de una pared, sin diálogo entendible |
| `AMB_Window_Rattle_01` … `04` | 4 | Un castañeo corto de marco o de vidrio |

Anotá autor, título, licencia y URL en el momento de descargar. Si lo dejás para el final, se pierde.

Pasá todo a 48 kHz, 24 bits. Loops en estéreo. One-shots en mono. Pico −3 dBFS. Carpeta `B`.

## Paso 3 — Avanzar el informe mientras esperás

Borrador de las secciones 2, 5.2 y 5.3. Dejá los decibeles en lápiz hasta probarlos en Wwise. Completá ya las filas de créditos de la librería: eso no depende del proyecto.

## Paso 4 — Importar y rutear

Abrí el proyecto solo después del mensaje de A. No cambies buses, sends, `Surface` ni los contenedores de pasos.

1. En `Interior`, dos sonidos en loop: `AMB_RoomTone_Loop` y `AMB_FridgeHum_Loop`. Output Bus = `Bus_Interior`. Loop infinito activado en el sonido.
2. En `Exterior`, `AMB_City_Glass_Loop`, loop, bus `Bus_Exterior`.
3. Si la calle suena de afuera de la casa y no detrás de un vidrio, un low-pass fijo y suave en ese sonido, en el Actor-Mixer. Ese filtro no se ata a ningún RTPC. El RTPC lo va a poner C en el bus.
4. En `Weather`: `AMB_Rain_Glass_Loop`, `AMB_Wind_Window_Loop`, y un Random Container `AMB_Window_Rattle`. Los tres a `Bus_Weather`. Lluvia y viento en loop.
5. En `Punctual`, un Random Container por familia: `AMB_Pipe`, `AMB_TV`, `AMB_Birds`, `AMB_Dog`, `AMB_Laugh`, `AMB_Car`. Importá los tomas. Cañería y tele a `Bus_Interior`. Pájaros, perro, risa y autos a `Bus_Exterior`.

Volumen de partida, para retocar de oído con impulsividad en 25: el tone apenas se oye, la heladera por debajo de la calle, la calle por debajo de un paso de A. Dispará `Play_Footstep` una vez para comparar. No regrabees sus archivos: bajá tu bus.

## Paso 5 — One-shots que no se repiten en bloque

En cada Random Container de `Punctual` y en `AMB_Window_Rattle`:

1. Play Mode = Continuous. Loop infinito.
2. Play Type = Shuffle.
3. Transition = Delay.
4. Silencio entre sonidos:

| Container | Mínimo | Máximo |
| --- | --- | --- |
| `AMB_Pipe` | 18 s | 45 s |
| `AMB_TV` | 35 s | 80 s |
| `AMB_Birds` | 12 s | 30 s |
| `AMB_Dog` | 25 s | 70 s |
| `AMB_Laugh` | 40 s | 90 s |
| `AMB_Car` | 20 s | 50 s |
| `AMB_Window_Rattle` | 4 s | 12 s |

La tele queda muy baja. Si se entiende una palabra, ese archivo sale.

## Paso 6 — Horario

En el volumen de `AMB_City_Glass_Loop`, clic derecho > Bind to State > `Bloque`:

| State | Volumen |
| --- | --- |
| `Manana` | −6 dB |
| `Mediodia` | 0 dB |
| `Tarde` | −3 dB |
| `Noche` | −12 dB |

Misma herramienta de Bind to State en los one-shots:

| Container | Se oye | El resto |
| --- | --- | --- |
| `AMB_Birds` | `Manana` a nivel bajo | −96 dB en los otros bloques |
| `AMB_Laugh` | Mañana, mediodía y tarde | −96 dB en `Noche` |
| `AMB_Dog` | Todos los bloques | En `Noche`, −6 dB extra |
| `AMB_Car` | Sobre todo `Noche`, a nivel claro | De día, muy bajo |
| `AMB_Pipe` | Siempre | Sin cambio de bloque |
| `AMB_FridgeHum_Loop` | Siempre | No lo ates a `Bloque` ni a la lluvia |

La heladera de noche tiene que quedar adelante de la calle sin subirla: la calle es la que baja.

## Paso 7 — Lluvia

En el volumen de `AMB_Rain_Glass_Loop`, Bind to State > `Precipitacion`:

| State | Lluvia |
| --- | --- |
| `Seco` | −96 dB |
| `Lluvia` | 0 dB |

En la ciudad, una segunda dimensión de state, o el ajuste manual si tu versión no acumula dos states en la misma propiedad: en `Lluvia`, 4 dB menos que el valor del bloque. En `Seco`, el valor del paso 6 solo.

Si las dos binds se pisan, dejá el bloque en la ciudad y creá un Actor-Mixer padre `City_Weather` cuyo volumen baje 4 dB solo en `Lluvia`. El hijo conserva el horario.

`AMB_Birds`: −96 dB cuando `Precipitacion` es `Lluvia`, además de la regla del bloque. Tienen que cumplirse las dos. Con una sola que falle, los pájaros cantan bajo la lluvia.

Viento y castañeo no dependen de `Precipitacion`. Existen con sol y con lluvia.

## Paso 8 — Viento

En el volumen de voz de `AMB_Wind_Window_Loop` y de `AMB_Window_Rattle`, Bind to RTPC > `IntensidadViento`. Dibujá la curva en estos puntos:

| RTPC | Volumen |
| --- | --- |
| 0 | −96 dB |
| 30 | −28 dB |
| 60 | −14 dB |
| 100 | −6 dB |

Con el RTPC en 0 el random sigue corriendo y no se oye. Al subir el slider, el próximo castañeo aparece solo. No hace falta atar el intervalo al RTPC.

## Paso 9 — Un solo evento de ambiente

1. Abrí `Play_Amb_Departamento`. Add Action > Play por cada loop y cada random continuo: tone, heladera, ciudad, lluvia, viento, castañeo, cañería, tele, pájaros, perro, risa, autos.
2. Abrí `Stop_Amb_Departamento`. Una acción Stop por cada uno de esos targets. Scope: lo que esté sonando de ese objeto.
3. Ninguno de estos sonidos lleva aux. Confirmá el Output Bus de cada uno. Si la lluvia se lava al poner `Recinto` en `Bano`, está llegando a `Bus_Foley` o tiene un send. Movela a `Bus_Weather`.

## Paso 10 — Prueba tuya

Dejá `Impulsividad` en 25 y `IntensidadViento` en 15. Lanzá `Play_Amb_Departamento` una vez y no lo cortes durante esta prueba.

1. `Manana` + `Seco`. Tiene que haber calle baja y, en algún momento, un pájaro. La heladera atrás.
2. Pasá a `Lluvia` y no toques nada más durante cuatro segundos. Entra el vidrio. Los pájaros se van. Volvé a `Seco` y esperá el cruce de vuelta.
3. En `Seco`, viento a 0: silencio de marco. Viento a 100: aire y, dentro de los 12 segundos, un castañeo. Volvé a 15.
4. `Mediodia`: la calle más presente. `Noche`: la calle se va y la heladera queda. `Play_Footstep` de A sigue oyéndose por encima de la cama de mediodía.
5. Con la lluvia andando, cambiá `Recinto` a `Bano`. La lluvia no tiene que ganar cola de azulejo. Si la gana, el bus está mal.

## Paso 11 — Informe

1. Corregí los decibeles del borrador si los moviste de oído.
2. Captura 4: las transiciones del State Group `Precipitacion` y, en la misma entrega, las de `Recinto` que ya dejó A. No reabras los sends para sacar la foto.
3. Créditos: tus tres grabaciones, más una fila por cada archivo o lote de librería.

## Mensaje de entrega a C

`Ambiente listo. Play_Amb_Departamento y Stop andan. Lluvia, viento y bloques probados. La lluvia no usa aux. Podés atar Impulsividad en Bus_Exterior y Bus_Interior. No cambies volúmenes de Bloque ni de Precipitacion.`

## Si sobra tiempo, después del paso 10

En `Fridge` y `Coffee`, bus `Bus_Foley`, así toman el estar:

1. `Play_Fridge_Open` y `Play_Fridge_Close`. Burlete y cierre. Mientras la puerta está abierta el hum puede bajar 2 dB, no apagarse.
2. `Play_Coffee`. Secuencia corta: pava o cafetera, verter, taza. Tres a cinco segundos, sin loop.

## No abras

`FS_Switch`, RoomVerb, sends, archivos de pasos y palmadas.

---

# C — Cuerpo, hechos del día y demo

En la corrección explicás la impulsividad y recorrés el Soundcaster.

Tu audio: pulso, respiración, celular, puerta, notebook, canilla, sábanas.
Tu Wwise: blend del pulso, curvas de `Impulsividad`, foley del día, orden de `SC_Demo_TP1`, PDF y zip.
Tu informe: secciones 1, 3, 5.5, 6 y 7. Capturas 1, 6 y 7.

Mientras A y B tienen el proyecto, hacé los pasos 1 a 5.

## Paso 1 — Pulso

1. Micro muy cerca de una almohada. Dos golpes suaves con el puño, lub y dub. Sin un sample de monitor clínico.
2. Armá un loop estable de cerca de 66 bpm: `Heart_Slow`. Otro de cerca de 112 bpm: `Heart_Fast`.
3. Ocho segundos alcanzan si el cruce del loop no hace click. Mono, seco, −3 dBFS.

## Paso 2 — Respiración

A 30 cm, fuera del eje del aire para que no sople el micrófono.

1. `FOL_Breath_In`. Una inhalación.
2. `FOL_Breath_Out`. Una exhalación.

## Paso 3 — Celular y notebook

Sobre una mesa de madera, micro a unos 10 cm.

1. `FOL_Phone_Buzz_01` a `03`. El celular vibrando de verdad. Cada toma dura menos de un segundo.
2. `FOL_Phone_Tick_01` a `03`. Un tick diminuto del parlante. Es el “escribiendo”.
3. `FOL_Phone_Notify_01` a `03`. Otro timbre, más agudo y más corto. Puede ser un sine de menos de 80 ms con una cola corta, si el ringtone real se parece al buzz. No tiene que sonar a logro.
4. `FOL_Laptop_Notify_01` a `03`. Más seco y más bajo que el notify del teléfono.

## Paso 4 — Puerta, con A o B

Micrófono de tu lado, puerta cerrada. La otra persona golpea con el nudillo, intensidad media, sin acelerar.

Grabá cada patrón entero en un archivo, tres veces si podés, y quedate con la mejor toma de cada uno:

1. `FOL_Knock_A`. Dos golpes separados por unos 180 ms, pausa de unos 800 ms, un golpe.
2. `FOL_Knock_B`. Tres golpes parejos, cada 220 ms.
3. `FOL_Knock_C`. Un golpe, pausa de 400 ms, dos más rápidos.

Tiene que oírse madera y un poco del otro lado. Esa sala ya viene en el toma. En Wwise este evento no lleva aux.

## Paso 5 — Canilla y cama

1. `FOL_Sink_Loop`. Ocho segundos de chorro medio, loopeable, mono, seco.
2. `FOL_Bed_01` a `03`. Un gesto corto de sábana por archivo.

Dejá todo en la carpeta `C`, 48 kHz, 24 bits, mono.

En paralelo, borrador de las secciones 1, 3 y 6 del informe. La 5.5 y la 7 las cerrás cuando la curva y la sesión estén probadas.

## Paso 6 — Pulso en Wwise

Abrí el proyecto solo después del mensaje de B.

1. En `Body`, un Blend Container `Heartbeat`, Output Bus `Bus_Body`. Confirmá que ese bus no tiene aux.
2. Adentro, `Heart_Slow` y `Heart_Fast`, los dos en loop.
3. En el Blend Track, el eje es `Impulsividad`. En 55 se oye el lento. En 100, el rápido. Entre medio, cruce.
4. Volumen del blend, Bind to RTPC `Impulsividad`:

| RTPC | Volumen |
| --- | --- |
| 0 | −96 dB |
| 50 | −96 dB |
| 55 | −96 dB |
| 70 | −16 dB |
| 100 | −8 dB |

5. `Play_Body_Heartbeat`: acción Play, loop, hacia `Heartbeat`.
6. `Stop_Body_Heartbeat`: acción Stop de ese container.
7. Probalo con el ambiente de B ya sonando. En 25 no hay pulso. En 60 empieza el lento. En 95 está el rápido, delante de la calle, sin tapar un paso. Si cansa el oído, el punto 100 pasa a −12 dB.
8. `Play_Body_Breath`: Sequence Container con inhala, una pausa breve y exhala. Bus `Bus_Body`. Un solo ciclo, sin loop.

## Paso 7 — La calle se apaga con la impulsividad

No cambies los volúmenes de `Bloque` ni de `Precipitacion`. Solo sumá el RTPC en el bus.

1. En `Bus_Exterior`, Voice Low-pass Filter, Bind to RTPC `Impulsividad`. En Wwise, 0 es abierto y 100 es cerrado.

| RTPC | Low-pass | Volumen del bus |
| --- | --- | --- |
| 0 | 0 | 0 dB |
| 70 | 25 | −4 dB |
| 100 | 50 | −8 dB |

2. En `Bus_Interior`, solo el low-pass: 0 cuando el RTPC está en 0, y 20 cuando está en 100. El volumen de la heladera no se toca.
3. Opcional, en `Bus_Weather`: de 70 a 100, bajar 3 dB. La lluvia se pone menos nítida y no desaparece.
4. `Bus_Body` no lleva este low-pass. El pulso queda seco también con el RTPC en 95.
5. Prueba: ambiente corriendo, subir de 25 a 60, a 75 y a 95. Bajar a 20 y esperar a que la calle vuelva. Esa vuelta es la demostración. Si no vuelve, la curva quedó plana o el bind está en el sonido de la ciudad en vez del bus, pisando el horario de B.

## Paso 8 — Celular

Todo en la carpeta `Phone`, Output Bus `Bus_Narrative`, sin aux.

1. Random Container `Phone_Buzz`, Shuffle, loop. Las tres vibraciones. Un silencio de unos 200 ms entre vueltas, para que no sea un motor continuo. Volumen físico, de mesa, no de alarma.
2. `Play_Phone_Buzz` lo dispara. `Stop_Phone_Buzz` lo corta.
3. Random Container `Phone_Typing`, Continuous, Shuffle, delay de 80 a 140 ms entre ticks. `Play_Phone_Typing` y `Stop_Phone_Typing`. El stop tiene que poder cortar en medio de la ráfaga.
4. Random `Phone_Notify`, Shuffle, un solo disparo, sin loop. `Play_Phone_Notify`.
5. Escuchalos contra el buzz. Si se confunden, el notify está grave o largo. Acortalo.

## Paso 9 — Puerta, notebook, canilla, cama

1. En `Door`, un Sequence Container por patrón, con el archivo del paso 4. Después un Random Container `Door_Knock` en Shuffle que elige el patrón, Continuous, y deja 1,8 a 2,2 s de silencio entre patrones.
2. `Play_Door_Knock` y `Stop_Door_Knock`. Bus `Bus_Narrative`. Muteá los aux y volvé a escuchar: el golpe casi no tiene que cambiar. Si cambia, tiene send.
3. `Laptop_Notify` en `Laptop`, Shuffle, un disparo, mismo bus narrativo, más bajo que el notify del teléfono. `Play_Laptop_Notify`.
4. `Sink_Water` en `Sink`, loop, Output Bus `Bus_Foley`. Así agarra el baño cuando el state está en `Bano`. `Play_Sink_Water`. En `Stop_Sink_Water`, fade de 0,3 s para que el chorro no corte en seco.
5. `Bed_Sheets` en `Bed`, Random Shuffle, un disparo, `Bus_Foley`. `Play_Bed_Sheets`.

## Paso 10 — Ordenar la sesión

En `SC_Demo_TP1`, este orden de arriba abajo:

1. `Play_Amb_Departamento`
2. `Play_Body_Heartbeat`
3. `Play_Footstep`
4. `Play_Test_Clap`
5. `Play_Phone_Buzz`, `Stop_Phone_Buzz`
6. `Play_Phone_Typing`, `Stop_Phone_Typing`
7. `Play_Phone_Notify`
8. `Play_Door_Knock`, `Stop_Door_Knock`
9. `Play_Laptop_Notify`
10. `Play_Sink_Water`, `Stop_Sink_Water`
11. `Play_Body_Breath`
12. `Play_Bed_Sheets`

Game syncs visibles: `Surface`, `Recinto`, `Precipitacion`, `Bloque`, `Impulsividad`, `IntensidadViento`.

Corré la pasada una vez solo, antes de llamar a los otros.

## Paso 11 — Pasada con los tres

Duración cerca de cuatro minutos. El punto 7 incluye tres segundos en los que nadie dispara nada.

1. `Play_Amb_Departamento` y `Play_Body_Heartbeat`. `Manana`, `Seco`, viento 15, `Dormitorio`, impulsividad 25, `Madera`. Tone, heladera atrás, calle baja, algún pájaro. Sin pulso.
2. Ocho pasos en madera, seis en `Alfombra`, seis en `Ceramico`, seis en `Porcelanato`.
3. Tres palmadas en `Bano`, tres en `Dormitorio`, tres en `Estar`.
4. `Lluvia`, esperar cuatro segundos, viento a 100, volver el viento a 15, volver a `Seco`.
5. `Mediodia`, `Tarde`, `Noche`. En noche, `Play_Bed_Sheets`. La heladera adelante. Impulsividad sigue en 25.
6. Volver a `Tarde` y a `Estar`. Impulsividad a 60, a 75 y a 95. Bajar a 20 y oír la calle volver.
7. `Dormitorio`, impulsividad 25. Buzz tres segundos, stop. Typing cuatro segundos, stop. Contar tres segundos de departamento vacío. Un solo notify.
8. `Estar`. Knock unos seis segundos, stop. Un notify de notebook. `Bano`, canilla cuatro segundos, stop. Una respiración.

Si un punto no se entiende sin señalar la pantalla, lo corrige el dueño: suelos y palmadas A, clima y horario B, pulso y foley C.

## Paso 12 — Cerrar la entrega

1. Secciones 1, 3, 5.5, 6 y 7 del informe, con la curva que quedó.
2. Captura 1: Actor-Mixer con las carpetas abiertas un nivel.
3. Captura 6: las curvas de `Impulsividad` en el pulso y en el low-pass de `Bus_Exterior`.
4. Captura 7: `SC_Demo_TP1` con los game syncs a la vista.
5. Pegá las capturas de A y de B en la sección 10.
6. Tus créditos: pulso, respiración, celular, puerta, canilla, sábanas.
7. Exportá el PDF.
8. Zip del proyecto con `Originals` y el `.wproj`. `.cache` y `.backup` pueden quedar afuera.

## Si sobra tiempo, después de una pasada limpia del paso 11

En `Bus_Foley`, salvo la exhalación: `Play_Couch`, `Play_Door_Open`, `Play_Door_Close`, `Play_Bag_Rustle`. En `Bus_Body`: `Play_Skill_Stop` (una exhalación) y `Play_Skill_Breath` (inhala cerca de 4, silencio cerca de 2, exhala cerca de 6, una sola vuelta). Sin voces.

## No abras

Contenedores de pasos, efectos RoomVerb, volúmenes de lluvia, volúmenes de `Bloque`, archivos de la carpeta de B.

---

# Lo que ninguno hace en este TP

Unity, música durante el día, voces, y el foley extra de cada uno si su prueba todavía no sale. El extra está al final de cada parte y solo empieza cuando el mensaje de entrega de esa parte ya se puede mandar.
