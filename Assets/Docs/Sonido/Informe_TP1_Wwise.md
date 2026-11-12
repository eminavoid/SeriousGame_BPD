# Trabajo práctico n.º 1
## Implementación sonora de una región de juego

Materia: Sonido y Música para Videojuegos
Fecha: 9 de octubre de 2026

Integrantes: Guidi, Poles, Cha

Proyecto: *MyS — Implementación sonora de una región de juego*
Región: el departamento de *Un Día Con Emociones (TLP)*

[Acá imagen 1 — proyecto abierto en Wwise]

## Descripción del proyecto

La región implementada es el departamento donde transcurre el juego. Es un serious game en primera persona: la persona jugadora pasa un día, de 07:30 a 23:00, dentro de un depto chico. Hay dormitorio, baño y un estar abierto a la cocina. No hay exterior jugable. La calle existe solo detrás de la ventana del dormitorio.

El personaje camina en medias. El departamento suena como una casa real: tone de cuarto, heladera, cañerías, tele del vecino, y una ciudad filtrada por vidrio. El clima se oye sobre esa ventana. El baño es otra sala. La impulsividad, el medidor del juego, hace entrar el pulso y apagar los agudos de la calle.

Esta entrega se demuestra en el Soundcaster. No está conectada a Unity. El juego todavía usa sonidos sintéticos de placeholder. Los eventos de este proyecto son los que el final de la materia va a disparar.

La sesión para probar la región es `SC_Demo_TP1`. Ahí se cambian el switch de suelo, los states de sala, clima y horario, y los RTPCs de impulsividad y de viento.

[Acá imagen 2 — sesión `SC_Demo_TP1` con el switch `Surface`, los states y los dos RTPCs a la vista]

## Técnicas de implementación

### 1. Personaje: pasos en distintas superficies, sin repetir el mismo toma

El personaje es una persona en su casa, recién despierta. Los pasos son de medias, no de botas. Hay cuatro suelos porque el departamento no es un solo material: madera en el dormitorio, alfombra en el sillón, cerámico en el baño y piso duro en la cocina. En el juego el mesh del piso es uno solo. El switch ya separa las zonas para que, en el final, un volumen de Unity cambie `Surface` al cruzarlas.

`Play_Footstep` dispara el Switch Container `FootSteps Switch`. El switch group es `Surface`.

| Switch | Container | Archivos |
| --- | --- | --- |
| `FS_Madera` | `Footsteps - Wood - Random` | 6 tomas de `footsteps wooden floor` |
| `FS_Carpet` | `Footsteps - Carpet - Random` | 6 tomas de `footsteps carpet floor` |
| `FS_Bath` | `Footsteps - Bath - Random` | 6 tomas de `footsteps bath floor` |
| `FS_Kitchen` | `Footsteps - Kitchen - Random` | 6 tomas de `footsteps kitchen floor` |

Cada rama es un Random Container. Un disparo elige una de las seis tomas de ese suelo. La reiteración se evita con variación de grabación: no hay un único archivo de paso pasado por un pitch. Seis apoyos distintos del mismo gesto alcanzan para que, al caminar, no vuelva siempre el mismo ataque. En el Soundcaster se aprieta `Play_Footstep` varias veces en cada switch para oírlo.

El switch sale por `Bus_Footsteps`. El evento es un paso por disparo. En el juego, más adelante, Unity lo va a postear según el movimiento, cerca de un paso cada medio segundo a 3,2 m/s.

[Acá imagen 3 — `FootSteps Switch`, el grupo `Surface` y un Random con sus seis tomas]

### 2. Ambiente: bioma, paisaje sonoro, clima y randomización

El bioma es un interior doméstico urbano. Se eligió el departamento, y no un bosque, porque es la única región del juego. Inventar un exterior recorrible hubiera desarmado la escena.

`Play_Amb_Departamento` lanza todas las capas. `Stop_Amb_Departamento` las corta. El paisaje sonoro usa los cuatro tipos que pide un interior vivido:

| Tipo | Qué se oye | Objeto |
| --- | --- | --- |
| Aire del cuarto | Tone de la pieza vacía | `AMB_RoomTone_Loop` |
| Máquina | Motor de heladera, cañería, tele del vecino | `AMB_FridgeHum_Loop`, `AMB_Pipe`, `AMB_TV` |
| Fauna y gente, detrás del vidrio | Pájaros, perro, risa, autos | `AMB_Birds`, `AMB_Dog`, `AMB_Laugh`, `AMB_Car` |
| Agua y viento | Lluvia sobre el vidrio, aire en el marco, castañeo | `AMB_Rain_Glass_Loop`, `AMB_Wind_Window_Loop`, `AMB_Window_Rattle` |

La ciudad de fondo es `AMB_City_Glass_Loop`. Está grabada o filtrada como calle oída desde adentro. La ventana del juego ya dice que afuera hay autos, alguien riendo y un perro: esos tres one-shots salen de ese texto.

Las camas quedan en loop. Los puntuales son Random Containers en Standard, cada uno con su propia espera, para que no caigan juntos: pájaros 8,5 s, cañería 18 s, tele 35 s, perro 25 s, risa 40 s, auto 20 s, castañeo 4 s. Esa es la randomización del ambiente. Un solo ladrido en loop se reconoce enseguida. Cuatro tomas, elegidas al azar y separadas de la tele y de la risa, no.

El clima es dinámico y se oye desde la habitación, no sobre la cámara. Hay dos controles porque el enunciado mezcla un cambio de escena y un cambio continuo:

- El state `Precipitacion` cruza de `Seco` a `Lluvia` en 2,5 s. En `Seco` la lluvia está a −92 dB. En `Lluvia` entra a nivel, la ciudad baja 4 dB y los pájaros quedan a −92 dB.
- El RTPC `IntensidadViento` (0–100, valor inicial 15) mueve el viento y el castañeo: 0 → −96 dB, 30 → −28 dB, 60 → −14 dB, 100 → −6 dB. El viento puede subir con sol o con lluvia.

El state `Bloque` (`Manana`, `Mediodia`, `Tarde`, `Noche`, transición de 3 s) no es la variable de gameplay. Reparte el mismo departamento a lo largo del día: los pájaros solo en la mañana, la risa apagada de noche, el perro 6 dB más bajo en `Noche`, los autos más presentes de noche, y la lluvia un poco más baja cuando el bloque es `Noche` (−12 dB). La heladera no sigue al reloj. De noche, con los pájaros y la risa afuera, el motor queda más adelante. Eso es el cierre del día, cuando el personaje se acerca a la cama.

[Acá imagen 4 — `Precipitacion` en `Seco` y en `Lluvia`, con la transición de 2,5 s]
[Acá imagen 5 — curva de `IntensidadViento`]
[Acá imagen 6 — `AMB_Birds` o `AMB_Pipe` con el Random y el state `Bloque`]

### 3. Recinto: states, reverb y buses

El recinto distinto es el baño. Es la única pieza cerrada y reflectante, y es donde ocurre la primera escena del día: el espejo. El dormitorio tiene cama y ropa, así que la cola es corta. El estar es el término medio.

El state group `Recinto` tiene `Dormitorio`, `Estar` y `Bath`. La transición es de 0,35 s para que el cambio de sala no sea un corte.

Cada valor alimenta un aux con Wwise RoomVerb. Los envíos están en `FootSteps Switch`, así el paso entra a la sala que corresponde:

| `Recinto` | `Aux_Reverb_Dormitorio` | `Aux_Reverb_Estar` | `Aux_Reverb_Bath` |
| --- | --- | --- | --- |
| `Dormitorio` | −12 dB | −108 dB | −108 dB |
| `Estar` | −108 dB | −8 dB | −108 dB |
| `Bath` | −108 dB | −108 dB | −2 dB |

−108 dB apaga ese aux. En cada state queda un solo reverb.

| Aux | RoomVerb |
| --- | --- |
| `Aux_Reverb_Dormitorio` | Decay 0,40 s, pre-delay 10 ms |
| `Aux_Reverb_Estar` | Decay 0,65 s, pre-delay 16 ms |
| `Aux_Reverb_Bath` | RoomVerb del baño. Es el envío más alto (−2 dB) y la sala que tiene que leerse como azulejo |

El dormitorio está a propósito más seco y más corto. El baño es el que se tiene que reconocer al cambiar el state, con el mismo suelo seleccionado, para no confundir la sala con el cambio de archivo del paso.

Los buses separan qué cosa tiene derecho a esa reverb:

| Bus | Contenido | Reverb de sala |
| --- | --- | --- |
| `Bus_Footsteps` | Pasos | Sí, por los aux del switch |
| `Bus_Foley` | Canilla | El state `Recinto` está asociado a este bus |
| `Bus_Body` | Pulso y respiración | No. Un corazón con cola de azulejo deja de ser cuerpo |
| `Bus_Narrative` | Celular, puerta, notebook | No. El golpe ya trae la puerta en el toma |
| `Bus_Interior` | Tone, heladera, cañería, tele | No |
| `Bus_Exterior` | Ciudad y fauna | No. Están detrás del vidrio. Además lleva el low-pass de la impulsividad |
| `Bus_Weather` | Lluvia, viento, castañeo | No. Si la lluvia entrara al RoomVerb del baño, el clima cambiaría al cruzar la puerta |
| `Bus_UI` | Reservado | No |

[Acá imagen 7 — Master-Mixer: buses y los tres aux]
[Acá imagen 8 — RoomVerb de `Aux_Reverb_Dormitorio` y de `Aux_Reverb_Estar`, con Decay y Pre-delay]
[Acá imagen 9 — sends del switch de pasos en `Dormitorio`, `Estar` y `Bath`]

### 4. Variable de gameplay

La variable es `Impulsividad`, de 0 a 100, valor inicial 25. Es el medidor del juego, no un slider decorativo. Por debajo de un umbral el cuerpo calla y la calle se oye ancha. Cuando el medidor sube, el pulso pasa adelante y el exterior pierde agudos. Bajar el RTPC devuelve la calle. En el juego, esa bajada es el efecto de parar o de bajar el cuerpo.

El pulso es el Blend Container `Blend Heartbeat`, en `Bus_Body`. Las dos capas salen de `Heartbeat.wav`, loopeado:

- `Heartbeat_Normal`, pitch −500 cents, ocupa el cruce entre 50 y 80.
- `Heartbeat_Fast`, pitch +500 cents, entra de 80 a 100.

El volumen del blend es:

| Impulsividad | Volumen |
| --- | --- |
| 0 | −200 dB |
| 50 | −10 dB |
| 70 | 0 dB |
| 100 | +7 dB |

En 25 el pulso todavía no está. Cerca de 50 aparece. Desde 70 queda a nivel, que es el punto en el que el juego ya considera el estado desregulado. Desde 80 la capa se acelera.

El mismo RTPC, en `Bus_Exterior`, sube el low-pass de 0 (en 0 y en 25) a 50 (en 100). En `Bus_Weather` el volumen baja 3 dB entre 70 y 100. La lluvia sigue, un poco más atrás. El pulso no pasa por ese low-pass: el cuerpo queda seco y la calle se apaga.

`Play_Body_Heartbeat` lo deja sonando para mover el slider. `Stop_Body_Heartbeat` lo corta.

[Acá imagen 10 — Blend Track: cruce Normal/Fast y curva de volumen contra `Impulsividad`]
[Acá imagen 11 — low-pass de `Bus_Exterior` ligado a `Impulsividad`]

## Cómo probarlo en el Soundcaster

Abrir `SC_Demo_TP1`. Tienen que verse el switch `Surface`, los states `Recinto`, `Precipitacion` y `Bloque`, y los RTPCs `Impulsividad` e `IntensidadViento`.

1. `Play_Amb_Departamento`. Dejar `Manana`, `Seco`, viento en 15, `Dormitorio`, impulsividad en 25, `FS_Madera`. Se oyen el cuarto, la heladera y la calle. El pulso no.
2. `Play_Footstep` varias veces en los cuatro switches. No tiene que ser siempre el mismo toma del mismo suelo.
3. Dejar `FS_Madera` y pasar `Recinto` a `Estar` y después a `Bath`. El archivo de paso es el mismo. Cambia la sala.
4. Pasar a `Lluvia` y esperar el cruce de 2,5 s. Subir `IntensidadViento` a 100 y bajarlo a 0. Volver a `Seco`.
5. Recorrer `Mediodia`, `Tarde` y `Noche`.
6. Lanzar `Play_Body_Heartbeat`. Subir `Impulsividad` a 50, a 80 y a 100. Bajarla a 25. El pulso entra y se va. Con el ambiente todavía activo, la calle pierde agudos en el tramo alto.
7. `Play_Phone_Buzz` y `Stop_Phone_Buzz`. `Play_Phone_Typing` y `Stop_Phone_Typing`. Dejar unos segundos de departamento solo. Un `Play_Phone_Notify`. Después `Play_Laptop_Notify`, `Play_Door_Knock` y `Stop_Door_Knock`, `Play_Sink_Water` y `Stop_Sink_Water`, `Play_Bed_Sheets`.

`Play_Body_Breath` cambia con el state `Gender` (`Boy` / `Girls`): son dos tomas del mismo gesto, para que la respiración tampoco quede fija en un archivo.

Si un evento de los del punto 7 no está todavía en la sesión, se arrastra desde Events a `SC_Demo_TP1`. El evento ya apunta al objeto correcto.

[Acá imagen 12 — la pasada del punto 6, con `Impulsividad` en un valor alto y el ambiente sonando]

## Decisiones

El departamento se mantiene en escala doméstica. Un mensaje sin respuesta es una vibración sobre la mesa, no una alarma. El golpe es de madera y va por `Bus_Narrative`, sin el reverb del baño. La urgencia de la tarde no tiene un sonido nuevo: es la misma impulsividad, porque en esa escena no hay un objeto. El silencio después de cortar el typing es parte del estímulo.

No hay voces ni música en el día. La heladera, la calle y el pulso ocupan ese lugar. Poner un tema encima le diría al jugador qué sentir. El juego pide que la emoción ya esté en el cuerpo y que la calle siga siendo una calle.

La lluvia no cae sobre el personaje. Cae sobre el vidrio. Por eso `Bus_Weather` no entra a los aux de sala. El baño cambia el paso. No cambia el clima.

## Archivos

Los que usan los eventos están en `Originals/SFX`.

| Uso | Archivos |
| --- | --- |
| Pasos | `footsteps wooden floor.wav`, `footsteps carpet floor.wav`, `footsteps ceramic floor.wav`, `footsteps kitchen floor.wav` |
| Cuerpo | `Heartbeat.wav`, `Breathing Boy.wav`, `Brething Girl.wav` |
| Celular y trabajo | `Vibrating Phone.wav`, `Cellphone Typing.wav`, `Cellphone Notification.wav`, `Laptop Notifiaction.wav` |
| Casa | `Door Knock.wav`, `Tap Water.wav`, `Opening Tap.wav`, `Sheets.wav` |
| Ambiente | `AMB_RoomTone_Loop.wav`, `AMB_FridgeHum_Loop.wav`, `AMB_City_Glass_Loop.wav`, `AMB_Rain_Glass_Loop.wav`, `AMB_Wind_Window_Loop.wav`, `AMB_Window_Rattle_01.wav`, más `AMB_Birds`, `AMB_Car`, `AMB_Dog`, `AMB_Laugh`, `AMB_Pipe` y `AMB_TV` |

[Acá imagen 13 — carpeta `Originals/SFX`]

## Qué queda afuera de esta entrega

Unity no entra en este TP. Cuando se conecte, los nombres no se cambian: `Play_Footstep`, `Surface`, `Recinto`, `Impulsividad`, `Play_Phone_Buzz`, `Play_Door_Knock`, `Play_Body_Heartbeat`, `Bloque`, `Precipitacion` e `IntensidadViento`.
