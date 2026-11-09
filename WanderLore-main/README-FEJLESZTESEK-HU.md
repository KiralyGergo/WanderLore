# WanderLore – fejlesztések

## Új funkciók
- Négy játékscene: főmenü, játék, szünet és győzelmi képernyő.
- Háromlétegű pályarajzolás: talaj/víz, gyümölcsök és süni, majd a fák lombkoronája.
- A süni a mozgásirányának megfelelően elfordul.
- A vízmezőn a mozgási sebesség a normál sebesség 50%-a.
- 10 gyümölcs összegyűjtése után győzelmi képernyő jelenik meg.
- Nappal–éjszaka színkorrekciós shader, fokozatos fényerő- és színátmenettel, napkelte/naplemente hangulattal.

## Irányítás
- WASD vagy nyilak: mozgás
- SPACE: gyorsítás (vízen is működik, de a víz lassító hatásával)
- P vagy ESC: szünet
- Szünetben ENTER vagy P: folytatás
- Szünetben M: főmenü
- Főmenüben ENTER: játék indítása
- Főmenüben Q vagy ESC: kilépés
- Győzelemkor ENTER: új játék, M: főmenü

## Font és shader
A meglévő `Content/Fonts/MenuFont.spritefont` fájlt nem módosítottam. A játék továbbra is ezt a tartalomnevet tölti be: `Fonts/MenuFont`.
A nappal/éjszaka shader forrása: `Content/Effects/DayNight.fx`, tartalomneve: `Effects/DayNight`.

## Megjegyzés
A shader egyszerű, teljes képernyős sprite-színkorrekciót használ (nappali/éjszakai tónus és meleg alkonyati árnyalat). Ez még nem külön bloom/glow utófeldolgozás és nem geometriai, árnyékot vető irányfény.
