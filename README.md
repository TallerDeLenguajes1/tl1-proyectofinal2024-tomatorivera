<section align='center'>
    <img src='https://i.postimg.cc/fbxPy1NQ/vbm-banner.png' border='0' alt='Voleyball Manager Banner'/>
</section>

<hr>
<p align="center">
  <img src="https://img.shields.io/badge/Desarrollador-@tomatorivera-orange"
       alt="badge de desarrollador">
  <img src="https://img.shields.io/badge/Plataforma-.NET_8-yellow?logo=dotnet&logoColor=yellow"
  	   alt="badge de plataforma">
</p>

<p align="center" style="margin: 15px 0;">
  <a href="#introducción">Introducción</a> •
  <a href="#instalación-y-ejecución">Instalación y ejecución</a> •
  <a href="#cómo-jugar">Cómo jugar</a> •
  <a href="#datos-técnicos">Datos técnicos</a> •
  <a href="#próximas-implementaciones">Próximas implementaciones</a>
</p>

[![vbm-portada.png](https://i.postimg.cc/NFY8B0gX/vbm-portada.png)](https://postimg.cc/62j4ht46)

<hr>

## Introducción

**Voleyball Manager** es un juego de consola en el que podés dirigir tu propio equipo de Voley! Crea tu propio equipo para comenzar a jugar partidos y progresar.

Contarás con una plantilla inicial de 14 jugadores, cada uno con distintas especialidades en el campo de juego _(Líbero, central, armador, punta y opuesto)_. Tendrás que poner en práctica tus habilidades como director técnico implementando estrategias adecuadas para poder ganar a los distintos equipos que se te presenten, cada uno con jugadores distintos y habilidades distintas.

## Instalación y ejecución

### Instalando el juego

Para descargar este juego, será necesario realizar la instalación de la plataforma **.NET 8** sobre la que fue desarrollada:
- [Instalación para Windows y MAC](https://dotnet.microsoft.com/en-us/download)
- [Instalación para Linux](https://learn.microsoft.com/es-es/dotnet/core/install/linux-ubuntu-install?pivots=os-linux-ubuntu-2204&tabs=dotnet8)

Luego, deberás clonar este repositorio utilizando **Git** o bien utilizando la opción **Download ZIP**.

Para clonar con *Git*, nos dirigimos en el cmd o bash a la carpeta donde queramos descargarlo, allí ejecutamos los siguientes comandos:

```bash
# Clonamos el proyecto
$ git clone https://github.com/TallerDeLenguajes1/tl1-proyectofinal2024-tomatorivera.git

# Ingresamos a la carpeta del mismo para proceder a ejecutarlo
$ cd tl1-proyectofinal2024-tomatorivera
```

O bien como se mencionó anteriormente, podés descargar el ZIP y descomprimirlo. Para ello, presionamos en este mismo repositorio el botón **<> Code** y a continuación **Download ZIP**. Una vez descargado el archivo comprimido, procedemos a descomprimirlo y podremos ejecutarlo.

### Realizando la ejecución

Para ejecutar el juego, debes dirigirte a la carpeta en donde está la carpeta de este repositorio, allí mismo abrir un cmd o bash y ejecutar los siguientes comandos:

```bash
# Ejecutamos el proyecto con .NET
$ dotnet run
```

> [!NOTE]
> Recuerda tener instalada la plataforma .NET 8

## Cómo jugar

### Creación y cargado de partidas

Al ingresar al juego, podrás ver el menú principal que te permitirá: **crear una nueva partida** o **cargar una ya creada**. 

El proceso de creación de una partida es simple: ingresamos nuestro nombre de DT *(nombre de usuario)* y a continuación el nombre de nuestro equipo *(o podemos dejar que se genere automáticamente con el nombre de algún equipo de voley Argentino)*.

Del mismo modo, para cargar una partida, seleccionaremos *'Cargar partida'* y veremos una lista de todas las partidas que hemos creado. Podremos identificar la partida que deseamos cargar mediante su Nombre de DT y fecha de creación.

### Opciones de juego

Una vez hayas ingresado en una partida, verás el **dashboard** de la partida con información relevante acerca de tu equipo e historial. Además, se te presentarán un menú con opciones útiles: *jugar partido amistoso*, *consultar plantilla de jugadores* y *consultar historial de partidos*.

Actualmente solo se encuentra disponible el modo de **partidos amistosos**.

### Jugando un partido

Al iniciar un partido, se desplegará la interfaz del **panel del partido** en la que podrás consultar información relevante como ser el *desarrollo del partido*, *marcador en tiempo real*, *detalles de tus jugadores suplentes*, *información del equipo rival* y *posición actual de los jugadores en cancha*.

El partido se desarrolla de manera automática basado en probabilidades que se calculan según varios factores, principalmente según las habilidades de los jugadores en la acción que realizan *(saque, remate, bloqueo, recepción y colocación)*. El cálculo de las probabilidades se explica más adelante.

A continuación del panel del partido y tras cada pelota parada, es decir, cuando se produzca un punto y/o un cambio de set, se mostrarán distintas opciones para que puedas inclinar las probabilidades a tu favor implementando las estrategias que creas convenientes. Entre ellas: *realizar una sustitución*, *consultar planilla de jugadores*, *continuar partido* o *abandonar partido*. Las sustituciones siguen las reglas de voley oficial, si un jugador A es reemplazado por un jugador B, entonces el jugador A solo puede reingresar en lugar del jugador B, además, tendrás 12 sustituciones en lugar de 6 por partido. Por otro lado, en la plantilla de jugadores podrás ver el detalle de tus jugadores titulares y suplentes.

Luego de cada partido, los datos del mismo son almacenados en el historial de la partida que puede ser consultado desde el dashboard.

## Datos técnicos

### Acerca del juego

Como se mencionó anteriormente, el desarrollo de los partidos es automático y basado en probabilidades, he aquí cómo se realiza el cálculo de ellas.

Existen situaciones de juego donde se deben medir las habilidades de dos jugadores para decidir el resultado de la acción. Por ejemplo, en una acción de saque, el resultado depende tanto de la habilidad de saque del jugador que lo realiza y la habilidad de recepción del jugador defensa al que se dirige la pelota, en estos casos, la diferencia se calcula como:

$diferenciaHabilidades = HabilidadSaqueJugadorA + BonificacionAccion - CansancioJugadorA - (HabilidadRecepcionJugadorB - CansancioJugadorB)$

De esta diferencia de habilidades se puede calcular la probabilidad de que la acción termine de una forma u otra. Siguiendo el ejemplo del saque, los resultados podrían ser que el receptor recepcione o no el saque. Las probabilidades de una u otra se miden de la siguiente manera:

$probabilidadAccion = 0.5 + (diferenciaHabilidades / 20)$

En este cálculo, se tiene en cuenta una probabilidad base de 50% de que suceda uno u otro resultado, mas/menos la diferencia de habilidades dividida en 20 (para que resulte un numero entre 0 y 1). De este modo, si el jugador que realiza un saque tiene más habilidades de saque que las habilidades de recepción que el jugador que recibe el balón, entonces tiene más probabilidades de hacer un punto directo.

Como se pudo observar en la primera fórmula, también entran en juego otros factores además de las habilidades, como la *bonificación de la acción* y el *cansancio del jugador*.

La **bonificación** de una acción solo se tiene en cuenta en casos particulares, como ser: 

- Si un jugador remata, puede recibir una bonificación en las probabilidades si el pase es bueno
- Si un jugador coloca para un rematador, puede recibir una bonificación en las probabilidades si la recepción anterior a la colocación es buena
- Si un jugador bloquea, puede recibir una bonificación en las probabilidades en caso de que el remate sea malo

Por otro lado, el **cansancio** del jugador es un atributo que aumenta tras cada participación del mismo en el partido, es decir, en cada pelota que toque su cansancio aumenta según la acción. Hay acciones que cansan menos como un *saque* y acciones que cansan más como un *remate* puesto que requieren más esfuerzo físico del jugador. El aumento del cansancio también tiene en cuenta el factor **experiencia** *(decimal del 0 al 10)* del jugador, por lo que, si el jugador tiene más experiencia, su cansancio aumenta más lento puesto que sabe gestionar mejor su esfuerzo. El cálculo de aumento del cansancio se realiza de la siguiente manera:

$cansancioJugador = cansancioJugador + \frac{incremento}{experiencia}$

El cansancio del jugador tiene un mínimo de 0 y un máximo de 10. Como vimos anteriormente, todas las probabilidades del jugador se ven afectadas por su cansancio, por lo que si el jugador está muy cansado, fallará más seguido. El cansancio se reestablece entre 1.5 y 2 puntos por cada *sustitución* y entre 0.5 y 1 punto tras cada *cambio de set*.

### Acerca del software

**Voleyball Manager** está desarrollado con C# .NET 8. Además, hace uso de librerías externas como:

- [Spectre Console](https://github.com/spectreconsole/spectre.console): Para el desarrollo del Front-End de la aplicación
- [NAudio](https://github.com/naudio/NAudio): Para el sistema de reproducción de audios 
- [Newtonsoft JSON](https://github.com/JamesNK/Newtonsoft.Json): Para el manejo de archivos JSON

*VBM* también utiliza algunos servicios web externos para generar ciertos datos durante la ejecución:

- [Random User API](https://randomuser.me): Para la generación de nombres de los jugadores
- [API Football](https://www.api-football.com): Para la generación de nombres de equipos y obtención de datos recientes de ligas y torneos de voley

> [!NOTE]
> VBM crea datos genéricos si es que no puede conectarse a los servicios web, por lo tanto, no se requiere conexión a internet para jugar

## Próximas implementaciones

Por el momento, VBM solo posee el modo de juego de partidos amistosos, sin embargo, hay muchas implementaciones pendientes en este desarrollo:

- Modo de juego de ligas
- Modo de juego de torneos
- Mercado de jugadores
- Sistema de entrenamiento de jugadores