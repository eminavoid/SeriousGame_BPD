using System.Collections.Generic;
using UnityEngine;

namespace SeriousGameBPD.Data
{
    /// <summary>
    /// Loads DayPlan from Resources if present; otherwise builds the MVP content in memory.
    /// </summary>
    public static class ContentFactory
    {
        public static DayPlan LoadOrCreateDayPlan()
        {
            // In-memory MVP content (reliable nested options / Spanish copy).
            return BuildEmbeddedDayPlan();
        }

        public static DayPlan BuildEmbeddedDayPlan()
        {
            var plan = ScriptableObject.CreateInstance<DayPlan>();
            plan.dayTitle = "Un día con emociones intensas";
            plan.dayIntro =
                "Vas a vivir un día ordinario… pero por dentro las emociones pueden subir como una marea. " +
                "No hay respuestas perfectas. El objetivo es notar qué pasa y probar herramientas para lidiar.";

            plan.skills = BuildSkills();
            plan.glossary = BuildGlossary();
            plan.events = BuildEvents();
            return plan;
        }

        static List<SkillDef> BuildSkills()
        {
            var stop = ScriptableObject.CreateInstance<SkillDef>();
            stop.id = SkillId.Stop;
            stop.displayName = "STOP";
            stop.shortDescription = "Frenar el impulso antes de actuar.";
            stop.howToUse = "Stop (paré) → Take a step back (darte un paso atrás) → Observe (observá) → Proceed mindfully (seguí con más claridad).";
            stop.successDelta = new AffectDelta(0, -20, 15);

            var tipp = ScriptableObject.CreateInstance<SkillDef>();
            tipp.id = SkillId.Tipp;
            tipp.displayName = "TIPP (lite)";
            tipp.shortDescription = "Bajar la urgencia cambiando el cuerpo un momento.";
            tipp.howToUse = "Temperatura, ejercicio intenso breve, respiración pausada y relajación muscular progresiva (versión simplificada).";
            tipp.successDelta = new AffectDelta(8, -28, 10);

            var opposite = ScriptableObject.CreateInstance<SkillDef>();
            opposite.id = SkillId.OppositeAction;
            opposite.displayName = "Acción opuesta / Chequear hechos";
            opposite.shortDescription = "Separar la emoción intensa de los hechos, y elegir una acción distinta al impulso.";
            opposite.howToUse = "¿Qué sé con certeza? ¿Qué estoy interpretando? Si la emoción no encaja del todo con los hechos, actuá en la dirección opuesta al impulso.";
            opposite.successDelta = new AffectDelta(5, -15, 20);

            return new List<SkillDef> { stop, tipp, opposite };
        }

        static List<GlossaryEntry> BuildGlossary()
        {
            var g1 = ScriptableObject.CreateInstance<GlossaryEntry>();
            g1.title = "¿Qué es el TLP?";
            g1.body =
                "El Trastorno Límite de la Personalidad (TLP o BPD) implica emociones muy intensas, " +
                "miedo fuerte al abandono, relaciones que oscilan, y a veces una sensación inestable de quién soy. " +
                "No es 'drama' ni manipulación: es un patrón de sufrimiento real. Muchas personas mejoran con apoyo y habilidades.";
            g1.relatedTag = EducationalTag.Desregulacion;

            var g2 = ScriptableObject.CreateInstance<GlossaryEntry>();
            g2.title = "Desregulación emocional";
            g2.body =
                "Las emociones suben más rápido, más alto y bajan más lento. Un comentario pequeño puede sentirse enorme. " +
                "Aprender a nombrar y modular esa intensidad es una de las claves del trabajo terapéutico (p. ej. DBT).";
            g2.relatedTag = EducationalTag.Desregulacion;

            var g3 = ScriptableObject.CreateInstance<GlossaryEntry>();
            g3.title = "Miedo al abandono";
            g3.body =
                "La idea de que alguien se va —o solo tarda en responder— puede activar pánico, enojo o vacío. " +
                "Eso no significa que la relación esté realmente terminando; a veces el sistema de alarma suena demasiado alto.";
            g3.relatedTag = EducationalTag.Abandono;

            var g4 = ScriptableObject.CreateInstance<GlossaryEntry>();
            g4.title = "Pensamiento blanco / negro";
            g4.body =
                "Todo o nada: 'me ama / me odia', 'soy lo mejor / no valgo nada'. Es una forma de simplificar el caos emocional, " +
                "pero deja poco espacio para el gris —y el gris suele ser donde están las relaciones reales.";
            g4.relatedTag = EducationalTag.BlancoNegro;

            return new List<GlossaryEntry> { g1, g2, g3, g4 };
        }

        static List<DayEvent> BuildEvents()
        {
            return new List<DayEvent>
            {
                EventMananaEspejo(),
                EventMananaMensaje(),
                EventMediodiaAmistad(),
                EventTardeUrgencia(),
                EventTardeTrabajo(),
                EventNocheCierre()
            };
        }

        static DayEvent EventMananaEspejo()
        {
            var e = ScriptableObject.CreateInstance<DayEvent>();
            e.eventId = "manana_espejo";
            e.block = DayBlock.Manana;
            e.title = "Mañana — El espejo";
            e.body =
                "Te despertás. Por un segundo no sabés bien cómo te sentís hoy ni 'quién' sos en este día. " +
                "El café ayuda un poco. El mundo afuera ya empezó sin vos.";
            e.tags = new List<EducationalTag> { EducationalTag.Identidad, EducationalTag.Vacio };
            e.enterDelta = new AffectDelta(-10, 5, -5);
            e.recommendedSkill = SkillId.Stop;
            e.options = new List<EventOption>
            {
                new EventOption
                {
                    optionId = "force_ok",
                    label = "Forzar una sonrisa y salir igual ('tengo que estar bien')",
                    delta = new AffectDelta(-5, 10, -8),
                    outcomeWithoutSkill = "Salís, pero por dentro hay un nudo. Como si llevaras una máscara que pesa.",
                    requiresSkillOffer = true,
                    suggestedSkill = SkillId.Stop,
                    outcomeWithSkill = "Parás un momento: 'esto es vacío e identidad incierta, no soy 'roto''. Salís con un poco más de aire.",
                    skillBonusDelta = new AffectDelta(5, -10, 12)
                },
                new EventOption
                {
                    optionId = "name_it",
                    label = "Nombrar en voz baja: 'hoy me siento difuso/a'",
                    delta = new AffectDelta(5, -5, 10),
                    outcomeWithoutSkill = "Ponerle nombre baja un poco la presión. No lo arregla todo, pero ya no estás solo/a con la niebla.",
                    requiresSkillOffer = false
                }
            };
            return BindWorld(e, WorldAnchorIds.Mirror, EventTriggerType.Pull, true, new AffectDelta(-3, 4, -3));
        }

        static DayEvent EventMananaMensaje()
        {
            var e = ScriptableObject.CreateInstance<DayEvent>();
            e.eventId = "manana_mensaje";
            e.block = DayBlock.Manana;
            e.title = "Mañana — Tres puntitos… y silencio";
            e.body =
                "Le escribiste a alguien importante. Vio el mensaje. Empezó a escribir. Dejó de escribir. " +
                "Pasaron veinte minutos. Tu pecho late como si fuera una emergencia.";
            e.tags = new List<EducationalTag> { EducationalTag.Abandono, EducationalTag.Idealizacion };
            e.enterDelta = new AffectDelta(-25, 30, -15);
            e.recommendedSkill = SkillId.Tipp;
            e.options = new List<EventOption>
            {
                new EventOption
                {
                    optionId = "spam",
                    label = "Mandar varios mensajes seguidos ('¿estás enojado/a conmigo?')",
                    delta = new AffectDelta(-15, 20, -20),
                    requiresSkillOffer = true,
                    suggestedSkill = SkillId.Tipp,
                    outcomeWithoutSkill = "La urgencia habla por vos. Después llega la vergüenza. La otra persona responde tarde: estaba en una reunión.",
                    outcomeWithSkill = "Usás TIPP un minuto: respirás, soltás hombros. Esperás. Cuando responde, el alivio es real —y no tuviste que incendiar el chat.",
                    skillBonusDelta = new AffectDelta(15, -25, 15)
                },
                new EventOption
                {
                    optionId = "wait",
                    label = "Esperar y distraerte un rato (aunque cueste)",
                    delta = new AffectDelta(5, -10, 8),
                    outcomeWithoutSkill = "Cuesta. El cuerpo sigue en alarma. Pero el silencio no era abandono: era la vida del otro."
                }
            };
            return BindWorld(e, WorldAnchorIds.Phone, EventTriggerType.Push, true, new AffectDelta(-4, 6, -4));
        }

        static DayEvent EventMediodiaAmistad()
        {
            var e = ScriptableObject.CreateInstance<DayEvent>();
            e.eventId = "mediodia_cafe";
            e.block = DayBlock.Mediodia;
            e.title = "Mediodía — Café con un amigo";
            e.body =
                "Tu amigo cancela a último momento: 'hoy no puedo, perdón'. " +
                "De golpe pasa de ser 'la mejor persona' a 'nunca le importé'. El gris desaparece.";
            e.tags = new List<EducationalTag> { EducationalTag.BlancoNegro, EducationalTag.Abandono, EducationalTag.Ira };
            e.enterDelta = new AffectDelta(-30, 25, -20);
            e.recommendedSkill = SkillId.OppositeAction;
            e.options = new List<EventOption>
            {
                new EventOption
                {
                    optionId = "cut_off",
                    label = "Cortar: 'no hace falta que nos veamos más'",
                    delta = new AffectDelta(-20, 15, -25),
                    requiresSkillOffer = true,
                    suggestedSkill = SkillId.OppositeAction,
                    outcomeWithoutSkill = "El impulso de todo-o-nada deja un vacío peor. Después dudás: ¿exageré?",
                    outcomeWithSkill = "Chequeás hechos: canceló un plan, no 'te borró de su vida'. Acción opuesta: 'ok, ¿reagendamos?'. Queda espacio para reparar.",
                    skillBonusDelta = new AffectDelta(20, -20, 25)
                },
                new EventOption
                {
                    optionId = "soft",
                    label = "Responder con decepción sin incendiar: 'me duele, espero otro día'",
                    delta = new AffectDelta(10, -10, 15),
                    outcomeWithoutSkill = "Duele igual, pero no quemás el puente. El vínculo puede sobrevivir a un mal mediodía."
                }
            };
            return BindWorld(e, WorldAnchorIds.Phone, EventTriggerType.Push, true, new AffectDelta(-5, 5, -5));
        }

        static DayEvent EventTardeUrgencia()
        {
            var e = ScriptableObject.CreateInstance<DayEvent>();
            e.eventId = "tarde_urgencia";
            e.block = DayBlock.Tarde;
            e.title = "Tarde — La urgencia en el pecho";
            e.body =
                "Estás solo/a. Una ola sube sin una causa clara: ira, tristeza, vacío. " +
                "Querés hacer algo YA para que pare —cualquier cosa. El cuerpo pide alivio inmediato.";
            e.tags = new List<EducationalTag> { EducationalTag.Desregulacion, EducationalTag.Vacio, EducationalTag.Ira };
            e.enterDelta = new AffectDelta(-35, 40, -25);
            e.recommendedSkill = SkillId.Tipp;
            e.options = new List<EventOption>
            {
                new EventOption
                {
                    optionId = "act_out",
                    label = "Actuar el impulso (gritar, romper algo chico, mandar un mensaje hiriente)",
                    delta = new AffectDelta(-10, -5, -30),
                    requiresSkillOffer = true,
                    suggestedSkill = SkillId.Tipp,
                    outcomeWithoutSkill = "Hay un segundo de descarga… y después más culpa y más soledad.",
                    outcomeWithSkill = "TIPP: agua fría en la cara, 20 sentadillas, respirar más lento. La ola sigue ahí, pero ya no te arrastra igual.",
                    skillBonusDelta = new AffectDelta(25, -35, 20)
                },
                new EventOption
                {
                    optionId = "ride_wave",
                    label = "Surfear la ola: cronometrar 10 minutos sin decidir nada grande",
                    delta = new AffectDelta(5, -15, 10),
                    requiresSkillOffer = true,
                    suggestedSkill = SkillId.Stop,
                    outcomeWithoutSkill = "Diez minutos duelen. La urgencia baja un poco sola. Aprendés: las olas pasan.",
                    outcomeWithSkill = "STOP te da permiso a no decidir en el pico. Observás la emoción como clima, no como verdad absoluta.",
                    skillBonusDelta = new AffectDelta(10, -20, 18)
                }
            };
            return BindWorld(e, WorldAnchorIds.Body, EventTriggerType.Push, true, new AffectDelta(-4, 8, -4));
        }

        static DayEvent EventTardeTrabajo()
        {
            var e = ScriptableObject.CreateInstance<DayEvent>();
            e.eventId = "tarde_feedback";
            e.block = DayBlock.Tarde;
            e.title = "Tarde — Un comentario en el trabajo / estudio";
            e.body =
                "Alguien dice: 'esto podría mejorar'. En tu cabeza se traduce a: 'soy un desastre, no sirvo'. " +
                "La vergüenza quema. Querés desaparecer… o demostrar que sos perfecto/a ya.";
            e.tags = new List<EducationalTag> { EducationalTag.BlancoNegro, EducationalTag.Identidad };
            e.enterDelta = new AffectDelta(-20, 20, -15);
            e.recommendedSkill = SkillId.OppositeAction;
            e.options = new List<EventOption>
            {
                new EventOption
                {
                    optionId = "collapse",
                    label = "Hundirse: 'siempre fallo, mejor ni intento'",
                    delta = new AffectDelta(-25, 5, -20),
                    requiresSkillOffer = true,
                    suggestedSkill = SkillId.OppositeAction,
                    outcomeWithoutSkill = "El blanco/negro te deja sin margen. Un feedback se vuelve una sentencia sobre tu valor.",
                    outcomeWithSkill = "Hechos: pidieron un ajuste, no tu expulsión. Acción opuesta: preguntar '¿qué parte priorizo?'. Recuperás agencia.",
                    skillBonusDelta = new AffectDelta(20, -15, 22)
                },
                new EventOption
                {
                    optionId = "ask",
                    label = "Pedir un ejemplo concreto y respirar antes de responder",
                    delta = new AffectDelta(8, -12, 12),
                    outcomeWithoutSkill = "Duele, pero el diálogo vuelve al terreno de lo concreto. Sos más que un comentario."
                }
            };
            return BindWorld(e, WorldAnchorIds.Laptop, EventTriggerType.Push, true, new AffectDelta(-3, 4, -3));
        }

        static DayEvent EventNocheCierre()
        {
            var e = ScriptableObject.CreateInstance<DayEvent>();
            e.eventId = "noche_cierre";
            e.block = DayBlock.Noche;
            e.title = "Noche — Revisar el día";
            e.body =
                "La casa está quieta. Repasás lo que pasó. Algunas cosas duelen. Otras las manejaste mejor de lo que creías. " +
                "Mañana puede ser otro clima emocional —y eso también es parte de vivir con emociones intensas.";
            e.tags = new List<EducationalTag> { EducationalTag.Identidad, EducationalTag.Desregulacion };
            e.enterDelta = new AffectDelta(5, -10, 10);
            e.recommendedSkill = SkillId.Stop;
            e.options = new List<EventOption>
            {
                new EventOption
                {
                    optionId = "ruminate",
                    label = "Rumiar todo lo 'malo' hasta quedarte sin sueño",
                    delta = new AffectDelta(-15, 10, -15),
                    outcomeWithoutSkill = "La noche agranda los fantasmas. El día se ve peor de lo que fue."
                },
                new EventOption
                {
                    optionId = "gentle",
                    label = "Anotar 1 cosa difícil y 1 cosa que hiciste bien (por mínima que sea)",
                    delta = new AffectDelta(15, -15, 20),
                    requiresSkillOffer = true,
                    suggestedSkill = SkillId.Stop,
                    outcomeWithoutSkill = "Hay espacio para lo difícil y para lo que sí cuidaste. Eso es claridad, no negación.",
                    outcomeWithSkill = "STOP al final del día: observás sin juzgarte del todo. Cerrás con un poco más de compasión hacia vos.",
                    skillBonusDelta = new AffectDelta(10, -10, 15)
                }
            };
            return BindWorld(e, WorldAnchorIds.Bed, EventTriggerType.Pull, true, new AffectDelta(-4, 5, -5));
        }

        public static DayEvent BuildDoorKnockEvent()
        {
            var e = ScriptableObject.CreateInstance<DayEvent>();
            e.eventId = "mediodia_puerta";
            e.block = DayBlock.Mediodia;
            e.title = "La puerta";
            e.body =
                "Alguien golpea. El pecho se aprieta: ¿es tu amigo? ¿vino igual? ¿es otra persona? " +
                "El miedo al abandono y la alarma se mezclan antes de saber quién es.";
            e.tags = new List<EducationalTag> { EducationalTag.Abandono };
            e.enterDelta = new AffectDelta(-12, 15, -8);
            e.recommendedSkill = SkillId.Stop;
            e.options = new List<EventOption>
            {
                new EventOption
                {
                    optionId = "open",
                    label = "Abrir: es el delivery del vecino. Se equivocó de puerta.",
                    delta = new AffectDelta(8, -12, 10),
                    outcomeWithoutSkill = "El cuerpo tarda en bajar. No era abandono: era una puerta equivocada."
                },
                new EventOption
                {
                    optionId = "ignore",
                    label = "No abrir. Quedarte quieto/a hasta que se vaya.",
                    delta = new AffectDelta(-10, 8, -8),
                    requiresSkillOffer = true,
                    suggestedSkill = SkillId.Stop,
                    outcomeWithoutSkill = "Los golpes paran. Queda la duda: ¿y si era alguien que sí importaba?",
                    outcomeWithSkill = "STOP: observás el miedo sin tener que resolverlo ya. Cuando paran los golpes, respirás. Podés chequear después.",
                    skillBonusDelta = new AffectDelta(8, -12, 12)
                }
            };
            return BindWorld(e, WorldAnchorIds.Door, EventTriggerType.Push, true, new AffectDelta(-3, 5, -3));
        }

        static DayEvent BindWorld(DayEvent e, string anchor, EventTriggerType type, bool canDefer, AffectDelta defer)
        {
            e.worldAnchorId = anchor;
            e.triggerType = type;
            e.canDefer = canDefer;
            e.deferDelta = defer;
            return e;
        }
    }
}
