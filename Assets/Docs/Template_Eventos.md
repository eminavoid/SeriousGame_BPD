# Template de eventos — Un Día Con Emociones (TLP)

Ficha de diseño para encargar un evento implementable. Quien la completa no necesita abrir Unity. Quien la recibe puede pasarla a `DayEvent` y al beat del día sin inventar texto, números ni puesta en escena.

El archivo clínico de los ocho eventos que ya se juegan está en `Catalogo_Eventos_Eleccion.txt`. El departamento, el TLP y el banco de ideas están en `Ideario_Entorno.md`. Esta ficha es el contrato entre diseño y programación.

---

## 1. Qué pasa cuando el evento entra

El jugador camina el departamento en primera persona. El reloj salta a la hora del evento, espera unos segundos y el evento queda pendiente.

| Disparo | Qué vive el jugador | Si no se acerca |
| --- | --- | --- |
| Pull | El objeto espera. Hay que caminar y apretar `E`. | Al vencer el plazo, el juego lo abre igual. |
| Push | El mundo interrumpe: vibra el celular, golpean, aparece un pensamiento, o sube una ola en la cámara. | Se puede postergar un rato. Cada ~4 s el estado empeora. Después se fuerza. |

Durante el evento se pausan el reloj y el movimiento. El panel muestra bloque, título, cuerpo, temas y botones de opción. Si el ancla es `phone`, en lugar del panel se abre el chat.

Elegir una opción aplica su delta. Si esa opción ofrece herramienta y el jugador la completa, se suman tres cosas, en este orden:

1. El delta de la opción.
2. El bonus de esa opción.
3. El efecto fijo de la herramienta (diseño no lo escribe; ya está en el juego).

| Herramienta | Efecto fijo al completarla |
| --- | --- |
| STOP | afecto 0 · impulsividad −20 · claridad +15 |
| TIPP (lite) | afecto +8 · impulsividad −28 · claridad +10 |
| Acción opuesta / chequear hechos | afecto +5 · impulsividad −15 · claridad +20 |

Si el jugador elige «seguir sin la herramienta» o cancela el minijuego, queda solo el delta de la opción y el texto sin herramienta.

Después se lee el desenlace y «Seguir». Ese texto vuelve a aparecer en el cierre del día, suelto, sin el cuerpo de la escena. Tiene que entenderse solo.

Ningún botón dice «correcto» o «incorrecto». El feedback es la consecuencia.

---

## 2. Lo que un evento puede pedir

Un evento nuevo usa lo que ya existe.

**Anclas.** Una sola por evento. El id va exactamente así:

| Id | Objeto | Dónde está | Qué puede hacer el mundo |
| --- | --- | --- | --- |
| `mirror` | Espejo del baño | Pared norte del baño | Pensamiento. No hay reflejo real. |
| `phone` | Celular | Mesa de luz, dormitorio | Vibra, beep, abre el chat. |
| `door` | Puerta de calle | Pared sur del estar | Golpea y se mueve apenas. Nadie entra. El jugador no sale. |
| `laptop` | Notebook cerrado | Mesa del estar | Pensamiento. Se le puede cambiar el texto de `E`. |
| `bed` | Cama | Dormitorio | Pensamiento. |
| `couch` | Sillón | Estar, sobre la alfombra | Pensamiento. La pareja, si aparece, es texto. |
| `sink` | Canilla | Bajo el espejo | Fuera de evento ofrece TIPP libre. En evento, manda el evento. |
| `window` | Ventana | Pared oeste del dormitorio | Pensamiento. La calle no se recorre. |
| `fridge` | Heladera | Cocina abierta | Pensamiento. |
| `coffee` | Cafetera | Mesada | Pensamiento. |
| `body` | Sin objeto | La cámara | Ola, viñeta, latido. Se abre solo, apenas queda pendiente. |

**Bloques.** `Manana` (antes de las 12:00), `Mediodia` (12:00 a 15:00), `Tarde` (15:00 a 20:00), `Noche` (desde las 20:00). La hora del reloj tiene que caer dentro del bloque.

**Temas.** De uno a tres, de esta lista. Son los únicos que el juego sabe nombrar:

| Id | Cómo lo lee el jugador |
| --- | --- |
| `Abandono` | miedo al abandono |
| `BlancoNegro` | pensamiento blanco/negro |
| `Vacio` | vacío |
| `Ira` | ira intensa |
| `Idealizacion` | idealización / devaluación |
| `Desregulacion` | desregulación emocional |
| `Identidad` | identidad inestable |

**Herramientas.** `Stop`, `Tipp`, `OppositeAction`, o ninguna. No hay una cuarta. Las tres están desde el inicio; el evento ofrece la que corresponde, no la desbloquea.

**Opciones.** Dos o tres. Cuatro aprietan el panel. Cada opción es una conducta, escrita como algo que el personaje hace o dice, no como un consejo al jugador.

**Puesta en escena que hoy no vive en el asset.** Orden en el día, hora, segundos de espera, segundos máximos de postergación, si el beat cuenta como situación del día, pensamiento al quedar pendiente, pensamiento si se evita, y —si es celular— el guion del chat. Van en la ficha igual. Sin eso, programación tiene que inventarlos.

Un evento de `body` no se posterga: al quedar pendiente se abre solo. Un evento de `phone` necesita burbujas; si no se escriben, el chat muestra el cuerpo de la escena como un pensamiento y nada más. Dos eventos pueden compartir ancla si ocurren en momentos distintos. El pensamiento de «está pasando» y el de «lo estás evitando» hoy están escritos por ancla, no por evento: si la misma ancla repite, la ficha trae las frases nuevas.

---

## 3. Cómo elegir la herramienta

Una herramienta por tipo de activación. Si dudás entre dos, gana la del cuerpo cuando la impulsividad de entrada pasa de +25.

| Si la escena es… | Herramienta | Ejemplo ya jugado |
| --- | --- | --- |
| El cuerpo está en emergencia. Hay que bajar la temperatura antes de pensar. | TIPP | Silencio del chat. Pedir el celular. Actuar la ola. |
| Hay un impulso y todavía no conviene actuar. La pausa es el aprendizaje. | STOP | Forzar la sonrisa. No abrir la puerta. Surfear la ola. Cerrar el día. |
| La emoción no cierra con los hechos, o actuarla empeora lo que importa. | Acción opuesta | Cortar al amigo. Hundirse por un feedback. Reclamarle a la pareja. |

La opción ya hábil no abre minijuego. Nombrar lo que pasa, esperar, pedir un dato concreto, quedarse en la charla: eso ya es la habilidad. Ofrecer el botón ahí convierte la escena en un examen.

El impulso (cortar, spamear, hundirse, reclamar, descargar) sí ofrece la herramienta. El jugador puede rechazarla.

STOP en el cierre de la noche es la excepción que ya existe: la herramienta está en la opción de cuidado, porque ahí hay que sostener una mirada sin juicio. No la copies a otras escenas de cuidado salvo que el impulso sea tapar, maquillar o decidir en el pico.

---

## 4. Cómo calibrar los tres números

Medidores, siempre visibles:

| Medidor | Rango | Arranque del día | Qué significa en juego |
| --- | --- | --- | --- |
| Afecto | −100 a +100 | 10 | El clima. Negativo duele; positivo no es «ganar». |
| Impulsividad | 0 a 100 | 25 | Las ganas de hacerlo ya. Desde 65 la cámara se cierra, tiembla y hay viñeta. Desde 70 es desregulación. |
| Claridad | 0 a 100 | 55 | Hecho separado de historia. Baja desde 35 se siente niebla. |

También hay desregulación si el afecto llega a ±70. Un evento puede buscar cruzar esos umbrales. No hace falta cruzarlos en todos.

El día aguanta un solo pico (clase crisis). El resto se reparte entre suave y alarma. El cierre entra un poco mejor que el resto: es la única hora en la que el cuerpo tiene una ventana para integrar.

### Clases de entrada

El delta de entrada se aplica al abrir, antes de elegir.

| Clase | Afecto | Impulsividad | Claridad | Cuándo |
| --- | --- | --- | --- | --- |
| Suave | −8 a −15 | +5 a +15 | −5 a −10 | Estímulo interno o señal ambigua. Espejo, puerta. |
| Alarma | −20 a −28 | +18 a +30 | −14 a −18 | Señal chica leída como enorme. Mensaje, feedback, pareja. |
| Crisis | −30 a −40 | +35 a +45 | −20 a −28 | Una sola por día. La ola del cuerpo. |
| Cierre | +5 | −8 a −12 | +8 a +12 | Solo la última escena del día. |

### Postergar

Cada ~4 segundos de evitación se aplica el delta de postergación. Con 20 s de plazo son unos cinco tics.

| | Afecto | Impulsividad | Claridad |
| --- | --- | --- | --- |
| Por tic | −3 a −5 | +4 a +8 | −3 a −5 |

Más grande que eso destruye el estado por caminar despacio. En `body` el diferir casi no se usa.

Plazos que ya funcionan: espera 6–12 s; postergación máxima 14–24 s. Pull de mañana y de noche, un poco más de espera (12 s). Push de una interrupción, menos (6–9 s).

### La opción

| Tipo de opción | Afecto | Impulsividad | Claridad |
| --- | --- | --- | --- |
| Impulso | −10 a −25 | sube (+5 a +28) | −15 a −30 |
| Descarga (gritar, romper, controlar) | −8 a −15 | puede bajar un poco (−5) | se hunde (−25 a −30) |
| Ya hábil, sin herramienta | +5 a +15 | −5 a −15 | +8 a +20 |

El camino hábil duele en el texto y alivia un poco en el medidor. No deja el afecto en +40.

### Bonus de la herramienta

Del mismo tamaño que el daño de esa opción, para compensarlo. Después el juego suma el efecto fijo de la tabla de la sección 1. Anotá las tres líneas en la ficha (delta, bonus, efecto fijo) y la suma, para ver si el total es creíble.

Un buen total con herramienta: la impulsividad baja de a saltos, la claridad recupera, el afecto vuelve hacia cero sin volverse euforia. Si la suma de impulsividad pasa de −50, el bonus está inflado: el efecto fijo ya hace una parte del trabajo.

Referencia real, opción «forzar la sonrisa» del espejo, con STOP:

| | Afecto | Impulsividad | Claridad |
| --- | --- | --- | --- |
| Delta de la opción | −5 | +10 | −8 |
| Bonus | +5 | −10 | +12 |
| Efecto fijo de STOP | 0 | −20 | +15 |
| Total al resolver | 0 | −20 | +19 |

---

## 5. Voz, largo y límites

El jugador lee en voseo, en segunda persona, en presente. Frases cortas. «Te despertás.» «El pecho late.» Sin nombre de personaje. Donde haga falta el género, el juego ya usa la barra: solo/a, difuso/a.

| Pieza | Largo | Qué hace |
| --- | --- | --- |
| Título | Una línea. Patrón: «Mañana — El espejo» | Situación, no moraleja. |
| Cuerpo | 2 a 4 oraciones. Unas 280–450 letras | El hecho y cómo se siente. Sin la palabra TLP, sin explicar la herramienta. |
| Etiqueta del botón | Una conducta. Menos de ~90 letras | Algo que se hace o se dice. |
| Desenlace | 1 a 3 oraciones | Qué pasó, y el fenómeno en criollo («el silencio no era abandono»). Se relee en el cierre. |
| Pensamiento pendiente | Una oración | Lo que cruza la cabeza cuando el objeto llama. |
| Pensamiento al evitar | Una oración | Evitar no apaga la alarma. |

El cuerpo no da la clase. El desenlace nombra la dificultad en una frase. El glosario largo queda para el cierre.

Límites de contenido, los mismos del juego de hoy:

- El hecho objetivo es chico: un visto, un plan, una frase, un golpe, una ola sin causa clara.
- El impulso jugable se queda en gritar, romper algo chico, mandar de más, cortar un plan, reclamar, revisar, hundirse, rumiar, no abrir, tapar el malestar.
- Quedan afuera del guion la autolesión, el suicidio, las sustancias, el abuso, el sexo y la crisis. Si la escena los necesita para funcionar, no es una escena de este juego.
- La pareja, si aparece, cuenta algo cotidiano. No hay ruptura, mensaje oculto ni pelea previa. Esa zona se valida con Psicología antes de entrar al build.
- No hay puntaje, diagnóstico, ni una opción marcada como la buena.

---

## 6. Ficha en blanco

Copiá desde aquí. Un archivo por evento. Los ids en `cursiva` son los nombres del código.

```
EVENTO
======
eventId:                 (minúsculas y _, único. ej. tarde_ventana)
Título:                  (Bloque — Nombre)
Bloque (block):          Manana | Mediodia | Tarde | Noche
Hora del reloj:          HH:MM  (tiene que caer en el bloque)
Orden en el día:         (después de cuál, antes de cuál)
Cuenta como situación:   sí | no
                         (no = beat de enseñanza, como la puerta. Hoy el flag existe y ninguna pantalla lo lee: dejar el motivo escrito.)

Ancla (worldAnchorId):   mirror | phone | door | laptop | bed | couch | sink | window | fridge | coffee | body
Disparo (triggerType):   Pull | Push
Espera antes de pending: __ segundos
Plazo para postergar:    __ segundos
Se puede postergar (canDefer): sí | no

Pensamiento al quedar pendiente:
Pensamiento si se evita:
Prompt de interacción (E), si cambia:   (si no cambia, dejar el de la casa)
Si es celular, guion del chat:
  Estado inicial:        (en línea / visto / vacío)
  Burbuja 1:             (quién: yo | otro | pensamiento)  texto / pausa en segundos
  Burbuja 2:
  Burbuja 3:
Si es body:              se abre solo y hay ola en la cámara

Temas (tags):            (1 a 3 ids)
Skill de la escena (recommendedSkill):  Stop | Tipp | OppositeAction | None
Por qué esa y no otra:

Cuerpo (body):
(2 a 4 oraciones. Hecho + sensación. Sin clase teórica.)

Hecho objetivo (una línea, para diseño):
Historia que arma la cabeza:
Impulso que pide la emoción:

Al entrar (enterDelta):
  afecto        __
  impulsividad  __
  claridad      __
Clase: suave | alarma | crisis | cierre

Si se posterga, por tic (deferDelta):
  afecto        __
  impulsividad  __
  claridad      __

Por qué este estímulo en TLP (para el equipo, no se muestra):
Criterio que ilustra:
Riesgo de activar de verdad, y cómo queda chico:


OPCIÓN A
--------
optionId:
Etiqueta (label):                 (la conducta)
¿Ofrece herramienta? (requiresSkillOffer):  sí | no
Herramienta (suggestedSkill):     Stop | Tipp | OppositeAction | None

Delta (delta):
  afecto        __
  impulsividad  __
  claridad      __

Desenlace sin herramienta (outcomeWithoutSkill):

Desenlace con herramienta (outcomeWithSkill):
  (vacío si no ofrece herramienta)
  Incluí el hecho concreto y la acción distinta al impulso.

Bonus si completa la herramienta (skillBonusDelta):
  afecto        __
  impulsividad  __
  claridad      __

Suma si la completa (delta + bonus + efecto fijo):
  afecto        __
  impulsividad  __
  claridad      __


OPCIÓN B
--------
(mismo bloque)


OPCIÓN C
--------
(borrar si son dos)


CIERRE DE LA FICHA
==================
Qué tiene que entender quien nunca vio el juego:
Qué no debe repetir de un evento ya existente:
¿Hace falta visto bueno de Psicología?  sí | no — por qué
```

---

## 7. Ejemplo lleno — el que ya está en el juego

Sirve para ver el grano. No lo reescribas: `manana_espejo` ya se juega.

```
EVENTO
======
eventId:                 manana_espejo
Título:                  Mañana — El espejo
Bloque:                  Manana
Hora del reloj:          07:36
Orden en el día:         1, antes del mensaje
Cuenta como situación:   sí

Ancla:                   mirror
Disparo:                 Pull
Espera antes de pending: 12
Plazo para postergar:    20
Se puede postergar:      sí

Pensamiento al quedar pendiente:
  El baño está ahí. El espejo también. ¿Quién mirás hoy?
Pensamiento si se evita:
  Lo que no mirás igual está en el cuerpo.
Prompt de interacción:   Mirarte en el espejo

Temas:                   Identidad, Vacio
Skill de la escena:      Stop
Por qué esa y no otra:
  El impulso no es descargar ni cortar un vínculo. Es tapar el malestar
  ("tengo que estar bien") y salir con una máscara. STOP entrena la pausa
  y el observar. TIPP sería demasiado cuerpo; todavía no hay emergencia.
  Acción opuesta llegaría antes de haber notado qué se siente.

Cuerpo:
  Te despertás. Por un segundo no sabés bien cómo te sentís hoy ni "quién"
  sos en este día. El café ayuda un poco. El mundo afuera ya empezó sin vos.

Hecho objetivo:          Es la mañana y no hay otra persona.
Historia que arma la cabeza:  No sé quién soy hoy. Si no me veo bien, estoy roto/a.
Impulso que pide la emoción:  Forzar una sonrisa y salir igual.

Al entrar:
  afecto −10 · impulsividad +5 · claridad −5
Clase: suave

Si se posterga, por tic:
  afecto −3 · impulsividad +4 · claridad −3

Por qué este estímulo en TLP:
  Sin el ancla de otra persona ni de un rol, la imagen de sí se siente
  inestable. El espejo hace concreta la pregunta. Forzar la sonrisa es
  invalidación propia; nombrar el estado es mindfulness.
Criterio que ilustra:    identidad inestable y vacío.
Riesgo de activar de verdad: bajo. No hay ruptura ni cuerpo en peligro.


OPCIÓN A
--------
optionId:                force_ok
Etiqueta:                Forzar una sonrisa y salir igual ("tengo que estar bien")
¿Ofrece herramienta?:    sí
Herramienta:             Stop

Delta:                   afecto −5 · impulsividad +10 · claridad −8
Desenlace sin herramienta:
  Salís, pero por dentro hay un nudo. Como si llevaras una máscara que pesa.
Desenlace con herramienta:
  Parás un momento: "esto es vacío e identidad incierta, no soy 'roto'".
  Salís con un poco más de aire.
Bonus:                   afecto +5 · impulsividad −10 · claridad +12
Suma (delta + bonus + STOP 0/−20/+15):
                         afecto 0 · impulsividad −20 · claridad +19

OPCIÓN B
--------
optionId:                name_it
Etiqueta:                Nombrar en voz baja: "hoy me siento difuso/a"
¿Ofrece herramienta?:    no
Herramienta:             None

Delta:                   afecto +5 · impulsividad −5 · claridad +10
Desenlace sin herramienta:
  Ponerle nombre baja un poco la presión. No lo arregla todo, pero ya no
  estás solo/a con la niebla.
Desenlace con herramienta: (no aplica)
Bonus:                   (no aplica)
```

---

## 8. Antes de entregar la ficha

- El `eventId` no choca con los que ya existen: `manana_espejo`, `manana_mensaje`, `mediodia_cafe`, `mediodia_puerta`, `tarde_urgencia`, `tarde_feedback`, `tarde_pareja`, `noche_cierre`.
- La hora cae en el bloque. El día de referencia arranca 07:30 y el último beat está en 21:12.
- Hay un hecho chico y una historia grande, y el desenlace los separa.
- Hay una opción que ya es hábil y no abre herramienta, salvo una escena de tolerancia al malestar en la que las dos vías necesiten apoyo (como la ola).
- La herramienta coincide con la tabla de la sección 3.
- Los números caen en una clase, y la suma con el efecto fijo está escrita.
- El desenlace se entiende en el cierre, sin el cuerpo de la escena.
- El texto está en voseo, sin «correcto», sin diagnóstico y sin contenido de crisis.
- Si hay pareja, ruptura, control o algo que pueda parecerse a la vida de quien juega con TLP, la ficha pide visto bueno de Psicología.
- La ficha dice el orden, la hora, los dos pensamientos y, si es celular, las burbujas.

Con eso programación carga el evento en el plan del día, agrega el beat y, si hace falta, el guion del chat y las frases del ancla. El greybox ya tiene los objetos: una ficha no pide un mueble nuevo.
