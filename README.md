# Touhou Ripoff ~ the Embodiment of My Ass

[In Swedish](./README.sv.md)

## Introduction

In this school project I had to build a simple game using Unity. I decided to
clone an already existing game instead of coming up with an (mostly) original
idea, as I am not that creative. At first I wanted to make a roguelike game,
inspired by The Binding of Isaac, but that turned out to be more work than I
had time to do. Finally I decided to make a bullet-hell or "danmaku" game, more
specifically a Touhou Project fangame. Touhou is a popular (at least in it's
niche) bullet-hell game franchise. The series creator, ZUN, has a very relaxed
attitude towards fangames, therefore there are a lot of them that I can take
inspiration from, were I to get stuck.

## Project Description

As stated, I used the popular game engine Unity to make this game. Unity is
popular because of its ease-of-use in making both games and other software and
it can compile to most modern platforms. In planning my project, I used a
textfile with a to-do list ([todo.txt](todo.txt)) in a simple kanban structure
(to do, doing and done). I also used three different importance levels marked
by an exclamation point for urgent, question mark for low-priority and nothing
for normal. In addition to the three standard kanban bins, I also had a list of
unfixed bugs sorted in the same importance levels.

## Result

The end product turned out as expected, all mechanics implemented and
functional, but lacking in level design. As stated in the introduction, I am
not a creative person and level design is not my forte. Had I more time to
work, I probably would have added more enemies and made the game longer,
however it would take time to come up with good placements for enemy attacks,
both spatial and temporal. In contrast to the creative aspects, the game
mechanics turned out good and easily expandable, aside from a few minor ones.
The biggest problem, not counting design, was storing enemy positions,
movements, and attack patterns. In the end I took inspiration from "Taisei
Project," an open source Touhou fangame. It used so-called "tasks," functions
set to run at specified times after the start of the game to spawn enemies et
cetera. These functions were defined programmatically in a "timeline" function,
which allowed for loops when timing multiple of the same task. The closest
analogue I found in Unity were coroutines, which was the obvious way to do it
from the beginning.

## Conclusion

In the end, the project went well. If I were to redo it, I would definitely try
to focus more on aesthetics and level design, i.e. the more creative part. I
would like to try to make my own textures instead of using free assets. And I
would like to have multiple longer levels, preferably with some sort of boss at
the end of each, much like the original Touhou games. If I ever have more time
in the future I would like to revisit this game and finish all of the things I
did not have time to make. But what I did have time to finish is still more
than enough for an introductory school project (I hope).
