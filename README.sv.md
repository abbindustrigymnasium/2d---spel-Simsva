# Touhou Ripoff ~ the Embodiment of My Ass

[På Engelska](./README.md)

## Inledning

I det här skolprojektet fick jag i uppgift att bygga ett enkelt spel i Unity.
Jag bestämde mig för att klona ett redan existerande spel istället för att
komma på en (mestadels) originell idé, eftersom jag inte är speciellt kreativ.
Från början ville jag göra ett roguelike spel, inspirerat av The Binding of
Isaac, men det visade sig innebära mer jobb än jag hade tid med. Till slut
bestämde jag mig för att skapa ett bullet-hell eller "danmaku" spel, mer
specifikt ett Touhou Project fanspel. Touhou är en populär (åtminstone inom sin
genre) bullet-hell spelserie. Seriens skapare, ZUN, har en mycket avslappnad
inställning till fanspel, och det finns därför många sådana spel jag kan ta
inspiration ifrån om jag skulle fastna.

## Projektbeskrivning

Som sagt använde jag den populära spelmotorn Unity i skapandet av mitt spel.
Unity är ett populärt val på grund av dess enkelhet, både i att skapa spel och
annan mjukvara. Det kan även kompilera till de flesta moderna platformarna. Jag
använde mig av ett textdokument med en att göra lista ([todo.txt](./todo.txt))
för att planera mitt projekt. Det är strukturerat i ett vanligt kanban format
(att göra, gör och gjort). Jag använde också tre olika viktighetsnivåer
markerat med ett utropstecken för brådskande, frågetecken för låg prioritet och
ingenting för normalt. Utöver de tre vanliga kanban kategorierna lade jag till
en kategori för ofixade buggar, sorterade i samma viktighetsnivåer.

## Resultat

Slutprodukten blev som förväntat, alla spelmekaniker implementerade och
fungerande, med med brist på nivåer och design. Som sagt i introduktionen är
jag inte en kreativ person och design är inte min styrka. Hade jag haft mer tid
att arbeta hade jag förmodligen lagt till fler fiender och gjort spelet längre,
däremot hade det tagit lång tid att placera och tidsbestämma fiender och
attacker bra. Till skillnad från den kreativa delen så blev spelmekanikerna bra
och lätta att utöka, i de flesta fallen. Det största problemet, bortsett från
design, var att lagra fiende positioner, rörelser och attacker. Jag tog
inspiration från "Taisei Project," ett Touhou fanspel med öppen källkod. Det
använde så kallade "uppgifter," tidsbestämda funktioner som skötte om fiender
och annat. Dessa funktioner definerades programmatiskt i en "tidslinje"
funktion, vilket tillåter loopar om man ska tidsbestämma flera av samma
uppgift. Den närmaste motsvarigheten jag hittade i Unity var "coroutines,"
vilket var det uppenbara sättet att lösa problemet från början om jag bara
hade tänkt mer på saken.

## Avslutning

I slutändan gick projektet bra. Skulle jag göra om det, skulle jag definitivt
fokusera mer på utseendet och designen, alltså den kreativa delen. Jag skulle
vilja försöka att göra mina egna texturer istället för att använda gratis
texturer jag hittat online. Jag skulle även gärna skapa längre nivåer, helst
med någon form av boss i slutet av varje, precis som i de originella Touhou
spelen. Om jag någonsin har mer tid i framtiden vill jag gärna återvända till
det här spelet och göra klart det jag inte haft tid att göra. Men det jag redan
har gjort är fortfarande mer än tillräckligt för en introduktion till Unity
(hoppas jag).
