# Steganographer
Program Steganographer schová zprávu do obrázku tak, že je téměř neodhalitelné,
že dochází ke komunikaci.

## Uživatelská dokumentace

### Záložka Encode
Po spuštění programu je uživatli zobrazena záložka "Encode". V ní se nachází
ovládací prvky pro schování zprávy do obrázku.

Horní textové pole je určeno pro zadání schovávané zprávy. Pod ním se nachází
progress bar indikující, jak moc je zprávou vyčerpána nosičová kapacita obrázku.
Nosičová kapacita obrázku záleží na zvolených parametrech a velikosti obrázku.

V levé části jsou prvky týkající se vstupního obrázku a vpravo výstupního.
Stisknutím tlačítka procházet uživatel vybere vstupní respektive výstupní
soubor.

Vlevo dole se nachází výběrové menu způsobu vyplňování pixelů. Na výběr je ze
tří možností: shora, zespodu a odprostřed. Na zvoleném místě bude v obrázku
schována zpráva.

Vpravo od způsobu vyplňování se nachází možnost "mezera". Ta odpovídá tomu jak
daleko od sebe budou pixely, v nichž se schovají bajty zprávy. Tuto mezeru lze
nastavit na hodnotu 0-254.

Při změně textu v horním textovém poli dojde k zakódování zprávy do výstupního
obrázku. Po stisknutí tlačítka uložit se výstupní obrázek uloží do nastaveného
výstupního souboru. Pokud je při stisknutí tlačítka uložit zpráva moc dlouhá na
uložení do obrázku, uživatel je upozorněn chybovou hláškou "The message is too
long".

### Záložka Decode

Záložka Decode je určena k přečtení schované zprávy z obrázku.

V horní části rozhraní je textové pole pouze pro čtení, ve kterém se ukáže
výsledná zpráva.

Pod ním je rozhraní pro zvolení vstupního souboru. Po jeho zvolení se ihned
zpráva zobrazí v textovém poli. Pokud se v obrázku zpráva nenachází nebo je
poškozena, uživatel je upozorněn chybovou hláškou "The image does not contain a
valid message".

## Technická dokumentace

Soubor `MessageHiderView.xaml` obsahuje rozhraní záložky Encode a soubor
`MessageRecoverView.xaml` obashuje rozhraní záložky Decode.

Po otevření obrázku uživatelem je obrázek zkopírován a převeden na formát Bgra32
třídou `ToWritableBitmapConverter` (pokud tento formát ještě nesplňuje).

Schování obrázku se odehrává v handleru události změny textu v textovém poli
`MessageTextChanged`. Ve formátu Bgra32 má každý pixel 32 bitů, tedy 4 bajty. Do
každého takového pixelu lze schovat jeden bajt zprávy pomocí statické třídy
`ByteHider` a to následujícím způsobem: bajt je rozdělen na 4 skupiny po dvou
bitech. Každá tato skupina je pak zapsána na řádově nejnižší pozice každého
jednotlivého bajtu v pixelu.

Například pokud se ve zprávě nachází bajt s hodnotou 27, neboli
**00**\_**01**\_**10**\_**11** a je právě zapisováno na pixel s hodnotou
0xAAAAAAAA, neboli 10101000\_10101001\_10101010\_10101011, výsledný pixel má
hodnotu 101010**00**\_101010**01**\_101010**10**\_101010**11**. Tímto je
zaručeno, že každý z kanálu R,G,B,A je pixelu změněn maximálně o hodnotu 3
(například z 255 na 252). To je pro lidské oko téměř neviditelné.

Do schovávaných dat jsou kromě textové zprávy schovány ještě další informace -
způsob vyplňování (`FillStart` -- jeden bajt), délka zprávy (ushort, tedy dva
bajty), mezera (`DataSpacing` -- jeden bajt) a kontrolní součet (v aktuální
verzi programu jeden bajt). Kontrolní součet je použit pro zkontrolování, zda se
v obrázku nachází zpráva, příp. jestli není poškozena (jinak kontrolní součet
nesedí).

Kontrolní součet je zobecněn abstraktní třídou `ChecksumCalculator`. V aktuální
verzi programu je použita implementace `XorChecksumCalculator`, která kontrolní
součet vypočítá jako XOR všech bajtů zprávy.

Zpráva je z obrázku přečtena opačným procesem od schovávání a to v handleru
změny zvoleného souboru `InputFileChoose_OnFilenameChanged`.

V řešení se také nachází projekt s unit testy - `Steganographer.Test`, který
obashuje třídy `ByteHiderTest` a `XorChecksumCalculatorTest`. V těch se nachází
unit testy pro schovávání a extrahování bajtu z pixelu a pro výpočet kontrolního
součtu.