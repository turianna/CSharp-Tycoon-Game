# Mini Transport Tycoon 

## Projekt leírás
**Háromfős csapatban** dolgoztunk a projekten, amely egy 2D-s játék fejlesztése volt C#.NET és WPF környezetben **MVVM architektúrát** alkalmazva. Így hatékonyan el tudtuk különíteni a játéklogikát a grafikus megjelenítéstől. A munkafolyamat **GitLab** környezetben folyt, ahol a verziókövetés érdekében alfeladatokra bontottuk a projektet, majd ezekhez elkülönített brancheket alkalmaztunk. A végleges program körülbelül **80%-os kódlefedettséggel** rendelkezik, **Unit tesztekkel** biztoítottuk az alapvető mechanikák és a peremesetek helyes működését is. A letisztult fejlesztés érdekében **automatizált CI pipeline-t** alkalmaztunk, mely minden commit után lefuttatta a build folyamatot.

## Személyes feladatok
A projekt során én feleltem a játékon belüli **vásárlási mechanika** fejlesztéséért, valamint az autók mozgását és útkeresését biztosító **BFS (Breadth-First Search) algoritmus** implementálásáért. Fejlesztés közben kiemelt figyelmet fordítottam a **Clean Code** alapelvek betartására és a kód átláthatóságára. Mindezek mellett aktívan részt vettem a további alapmechanikák programozásában és a Unit tesztek írásában is.

## Projekt futtatása
- Repository klónozása és a .sln fájl megnyitása Visual Studióban.
- A Solution Explorerben jobb gombbal kattintás a WPF projektre, és a "Set as Startup Project" lehetőség kiválasztása.
- A játék elindítható (F5).

## Játék leírás
A játék egy 2D-s felülnézetes térképen játszódik, amelyen városok, ipari létesítmények, az
ezeket összekötő úthálózat és terepi objektumok találhatóak. A játékos utakat építhet, megállókat
helyezhet el az úthálózaton, közúti járműveket vásárolhat útvonalakat definiál és ezek segítségével
árukat és/vagy utasokat szállíthat. A játék valós időben fut, de az idő múlása gyorsítható.

## Játékbemutató Videó
https://www.youtube.com/watch?v=yFw-ULrpP8U

## Játékos feladatai

- Útépítés
- Megállók lehelyezése
- Járművek vásárlása
- Hidépítés
- Terraformálás

## Plusz feladatok
- Erdők [0.5 komplexitás] 
- Perzisztencia [0.5 komplexitás]
- Folyók és tavak [0.5 komplexitás]
- Domborzat [1 komplexitás]
- Többhozzávalós termékek (saját feladat) [0.5 komplexitás]

### Többhozzávalós termékek [0.5 komplexitás]:
Vezessük be a termeléshez több alapanyagot igénylő gyárakat. 
A több alapanyagot igénylő gyárak csak minden alapanyag megléte után legyenek képesek a késztermékük gyártására. 
A késztermékek igényelhessenek különböző mennyiséget adott alapanyagból (például 2 fa és 1 kő). 
A már leszállított alapanyagokat a gyárak legyenek képesek eltárolni, majd minden szükséges alapanyag megérkezése után felhasználni azokat. 
A többhozzávalós késztermékek érjenek többet az általános egyhozzávalós késztermékeknél. Legalább egy ilyen gyár legyen.


## Képek a játékról

### Kezdőképernyő
![Kezdőképernyő](kezdokepernyo.png)

### Gombok vásárláshoz
![Vásárlás gombok](vasarlas.png)

### Gombok terraformáláshoz
![Terraformálás gombok](terraform.png)

### Álló és mozgó jármű listák
![Járművek listái](autolista.png)

### Játék közben
![Játék közben](game.png)

A képek a játékhoz AI segítségével készültek.
