# Plan de trabajo — TP1 Wwise

Materia: Sonido y Música para Videojuegos
Entrega: 9/10
Juego: *Un Día Con Emociones — TLP*
Región: el departamento, de 07:30 a 23:00

Este plan es para armar el proyecto de Wwise y el informe. La integración en Unity queda para el final de la materia. Los nombres de eventos, states, switches y RTPCs de la biblia son el contrato entre las dos entregas: si Wwise los respeta, el final es conectar, no reinventar.

Documentos:

- `Biblia_Sonora_Departamento.md` — especificación para diseñar y construir el audio sin haber jugado.
- `Informe_TP1_Wwise.md` — texto para entregar. Justifica las decisiones. Completar créditos y capturas cuando el proyecto exista.
- `Reparto_TP1.md` — las mismas tareas en tres partes, una por integrante.
- Este archivo — orden de trabajo y qué no se corta si falta tiempo.

## Qué se entrega

1. Zip del proyecto Wwise, con la carpeta `Originals` y el archivo `.wproj`. Se puede omitir `.cache` y `.backup`: Wwise los regenera al abrir.
2. Informe exportado a PDF. El anexo de producción es la biblia.

El profesor evalúa dentro del Soundcaster. No hace falta un build del juego.

## Tesis de una frase

Un departamento real, escuchado desde adentro, donde los objetos suenan chicos y la impulsividad agranda el cuerpo y apaga la calle.

Eso cubre la consigna sin inventar un bioma que el juego no tiene:

| Requisito del TP | Dónde vive en el proyecto |
| --- | --- |
| Pasos en distintas superficies, sin repetición | Switch `Surface`: Madera, Ceramico, Porcelanato, Alfombra. Random Shuffle + randomizer de pitch y volumen |
| Bioma y paisaje sonoro | Interior doméstico + ciudad filtrada por la ventana. Heladera, cañerías, perro, risa, tráfico |
| Clima dinámico | State `Precipitacion` (Seco / Lluvia, cruce de 2,5 s) y RTPC `IntensidadViento` (0–100) |
| Recinto con states y reverb | State `Recinto`: Dormitorio, Estar, Bano. Aux con RoomVerb. El baño es el recinto distinto |
| Variable de gameplay | RTPC `Impulsividad` (0–100): pulso y low-pass de la calle |
| Soundcaster | Sesión `SC_Demo_TP1`, con el guión de la biblia |

## Qué entra en el TP y qué queda afuera

Entra: todo lo que se puede demostrar apretando eventos, states y sliders en el Soundcaster.

Queda para el final: triggers de habitación en Unity, posicionamiento 3D, oclusión, el intervalo real de los pasos (el personaje camina a 3,2 m/s), reemplazar los placeholders de `ProceduralAudio` (beep, knock y latido senoidal), y un tema solo para la escena de debrief.

No entra ni ahora ni como efecto barato: voces actuadas, música durante el día, gritos, cristales, alarmas de “crisis”. La escala es doméstica. El detalle está en la biblia, sección de criterio.

## Orden del día

Si solo hay un día, seguir este orden. Cada bloque deja el proyecto en un estado que ya se puede abrir y escuchar.

### 1. Grabación seca — 90 min

Lista P0 de la biblia, sección Grabación. Un paso por archivo, micro cerca, sin reverb de la sala cocinada en el toma. La reverb del baño la pone Wwise: si el archivo ya suena a azulejo, el state no se nota.

Prioridad de grabación: pasos en las cuatro superficies, tres palmadas secas, vibración del celular sobre madera, golpes del otro lado de una puerta, canilla, tone de cuarto, motor de heladera, calle con la ventana cerrada.

Lluvia, viento, pájaros y perro pueden ser de librería CC0 si no se consiguen hoy. Anotar autor, título, URL y licencia en el informe antes de cerrar el zip.

### 2. Esqueleto vacío — 60 min

Crear el proyecto y, antes de importar audio, dejar armados:

- Work units y carpetas de la biblia, sección Arquitectura.
- Buses y aux de reverb, aunque el efecto todavía esté en preset.
- States `Recinto`, `Precipitacion`, `Bloque`.
- Switch `Surface`.
- RTPCs `Impulsividad` e `IntensidadViento`.
- Eventos con el nombre final, aunque apunten a silencio.
- Sesión `SC_Demo_TP1` con los game syncs ya visibles.

La nota de presentación (20 %) se juega acá. Un proyecto ordenado con audio simple puntúa mejor que un proyecto brillante con nombres `Audio_final_FINAL`.

IDs en ASCII, sin tildes: `Bano`, `Ceramico`, `Manana`.

### 3. Pasos — 90 min

`Play_Footstep` → Switch Container → cuatro Random Containers en Shuffle, seis archivos cada uno si hay, mínimo cuatro. Randomizer de pitch ±45 cents y volumen de −2,5 dB a 0 dB.

Probar en el Soundcaster disparando seguido. Si se reconoce el mismo toma dos veces seguidas, no está listo.

### 4. Ambiente, bloque horario y clima — 2 h

Un solo evento `Play_Amb_Departamento` que dispara todas las capas. Volúmenes por state según las tablas de la biblia. La lluvia es sobre el vidrio, no sobre el jugador. El cruce Seco → Lluvia dura 2,5 s y se tiene que oír.

### 5. Reverb — 60 min

RoomVerb en los tres aux. Sends solo en `Bus_Footsteps` y `Bus_Foley`. El cuerpo, la calle, la lluvia y la UI no entran al aux. Probar con `Play_Test_Clap`: baño, dormitorio, estar, en ese orden. El baño se tiene que reconocer en un segundo.

### 6. Impulsividad — 60 min

Loop de pulso lento y loop de pulso rápido en un Blend Container. Volumen en silencio por debajo de 55 y presente desde 70. Voice Low-pass del bus exterior sube con el mismo RTPC. Bajar el slider tiene que devolver la calle: esa vuelta es la demostración de que la variable está viva.

### 7. Foley mínimo para el relato — 45 min

Buzz del celular, ticks de “escribiendo”, notify distinto, golpe de puerta en loop con `Stop`, notify del notebook, canilla. Con eso el Soundcaster cuenta el día. Café, heladera abierta, sábanas, sillón y habilidades quedan en P2: suman credibilidad si el resto ya funciona.

### 8. Pasada del guión — 45 min

Correr `SC_Demo_TP1` de arriba abajo sin tocar la jerarquía. Anotar lo que no se entiende a la primera y corregir solo eso.

### 9. Informe, capturas y zip — 60 min

Capturas pedidas al final del informe. Completar la tabla de créditos con lo que realmente se usó. Exportar PDF. Zip.

## Definición de terminado

El TP está listo cuando una persona que no vio el juego puede:

1. Abrir `SC_Demo_TP1`.
2. Cambiar de superficie y no escuchar el mismo paso repetido.
3. Pasar de seco a lluvia y oír el cruce, y mover el viento de 0 a 100.
4. Cambiar a `Bano`, tirar la palmada y oír otra sala.
5. Subir `Impulsividad` de 25 a 90 y oír el pulso entrar y la calle apagarse.
6. Disparar el celular, dejar el silencio, y disparar la puerta.
7. Leer en el informe por qué cada una de esas decisiones sigue al departamento y no a un demo genérico de Wwise.

## Si falta tiempo, esto no se corta

Se corta primero el foley P2. No se corta la nomenclatura, los pasos, el cruce de lluvia, el aux del baño, la curva de impulsividad, ni el párrafo del informe que explica por qué el corazón y la lluvia no llevan la reverb del baño.

## Puente al final

Cuando el TP esté entregado, no renombrar eventos “para que queden más lindos”. Unity va a llamar estos nombres. La tabla de enlace está al final de la biblia.
