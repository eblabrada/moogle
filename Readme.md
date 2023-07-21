# Moogle!

> Proyecto de Programación I.
> Facultad de Matemática y Computación - Universidad de La Habana.
> Curso 2023.

## Introducción

Moogle! es una aplicación *totalmente original* cuyo propósito es buscar inteligentemente un texto en un conjunto de documentos.

Es una aplicación web, desarrollada con tecnología .NET Core 6.0, específicamente usando Blazor como *framework* web para la interfaz gráfica, y en el lenguaje C#.
La aplicación está dividida en dos componentes fundamentales:

- `MoogleServer` es un servidor web que renderiza la interfaz gráfica y sirve los resultados.
- `MoogleEngine` es una biblioteca de clases donde está... ehem... casi implementada la lógica del algoritmo de búsqueda.

## Motor de búsqueda

El motor de búsqueda usa un modelo vectorial que nos computa para una `query` dada qué tan relevante es un documento determinado. Este modelo vectorial usa `Term Frequency and Inverse Document Frequency (TF-IDF)` con `Cosine Similarity` para computar la relevancia. 

Para computar el vector `TF-IDF` uso la fórmula:

$$TFIDF = (\frac{tf}{tw}) \times \ln(\frac{td}{dt})$$

En esta fórmula tenemos que:
 * $tf$ es la frecuencia del término en el documento actual.
 * $tw$ es la cantidad de palabras totales en el documento actual.
 * $td$ es la cantidad total de documentos a analizar.
 * $dt$ es la cantidad de documentos que contienen el término.

> **Nota importante**: dado que $\frac{td}{dt}$ puede causar problemas por la división entre $0$ decidí que si $dt = 0$ luego $TFIDF = 0$, tiene sentido hacer esto ya que si $dt = 0$ el término no aparece en ningún documento.

Después de hacer lo anterior necesitamos calcular la "similitud" entre el vector de `document` y el vector de la `query`, que para eso utilizo `Cosine Similarity`. 
Para hacer esto intentamos estimar el "ángulo" comprendido entre el vector de la `query` y el vector de `document`: mientras menor sea este ángulo, mayor "similitud" tendrán estos vectores. Esto lo hacemos usando la fórmula:

$$\cos \alpha = \frac{v_d \cdot v_q}{||v_d|| ~ ||v_q||}$$

Siendo:
 * $v_d$ el vector de `document`
 * $v_q$ el vector de `query`
 * $||v||$ es la magnitud del vector $v$

## Interfaz gráfica

Lamentablemente la interfaz sigue siendo bastante pobre. Solo tuve tiempo para modificar pocas cosas en la interfaz. 

> Nota: La verdad es que estoy esperando encontrar un "socio" en la... ehem... (ya saben 🙃) que me ayude con esta parte.

Por ahora, mientras mi búsqueda de ese "socio" no ha dado resultado; solo he arreglado algunas cosas estéticas a la hora de mostrar los resultados en la búsqueda, y en el botón de búsqueda y en la `input` de la `query`. También corregí un pequeño, pero molesto; error a la hora de escribir "Quisiste" en la sugerencia.

## Sobre el contenido a buscar

La idea original del proyecto es buscar en un conjunto de archivos de texto (con extensión `.txt`) que estén en la carpeta `Content`. Estos documentos deben estar escritos en formato UTF-8 y deben estar **preferentemente** en inglés.

## Ejecutando el proyecto

Lo primero que tendrás que hacer para poder trabajar en este proyecto es instalar `.NET Core 6.0`. Luego, solo te debes parar en la carpeta del proyecto y ejecutar en la terminal de Linux:

```bash
make dev
```

Luego abrir en tu navegador: [Moogle!](http://localhost:5000), luego escriba su búsqueda en la barra de búsqueda y haga click en el botón *Buscar*

Si estás en Windows:
 1. ~~Instala Linux~~
 2. ~~Instala WSL (Windows Subsystem for Linux)~~
 3. Deberías poder ejecutar este proyecto usando: 
 
 ```bash
 dotnet watch run --project MoogleServer
 ```
 
> Nota aclaratoria: no se asegura el mismo "performance" de este proyecto en Windows, pero debería funcionar todo sin problema de ningún tipo.

## Funcionalidades adicionales

Están implementados y se pueden usar sin problemas de ningún tipo los siguientes operadores:

 * `!`: delante de una palabra este símbolo indica que esa palabra **no debe aparecer** en ningún documento que sea devuelto.
 * `^`: delante de una palabra este símbolo indica que esa palabra **debe aparecer** en cualquier documento que sea devuelto.
 * `~`: entre dos o más términos este símbolo indica que esos términos deben aparecer cerca, o sea, que mientras más cercanos estén en el documento mayor será la relevancia.
 * `*`: cualquier cantidad de símbolos `*` delante de un término indican que ese término es más importante, por lo que su influencia en el `score` debe ser mayor que la tendría normalmente (este efecto será acumulativo por cada `*`)
 
> Nota: Es importante tener en cuenta que para que estos operadores tengan algún efecto en los resultados deben ser usados correctamente. 
> * En el caso de los operadores `!`, `^` y `*` deben aparecer como prefijo de la palabra. Por ejemplo, `!word`, `^word`, `****word` usan correctamente estos operadores; pero `! word`, `^ word`, `**** word` no lo usan correctamente.
> * En el caso del operador `~` se requiere que esté separado de las palabras a las que se le aplica la operación, por ejemplo, `word1 ~ word2 ~ word3`, `word1 ~ word2`, son usos correctos de este operador, pero `word1~word2~word3`, `word1~word2` no son usos correctos de este operador. 
> * No se debe usar dos operadores para una misma palabra al mismo tiempo, por ejemplo `**word1 ~ !word2`, `!*****word`, `^*word`, `!^word` son incorrectos usos de los operadores.
 
## Otras funcionalidades

Se le harán determinadas sugerencias de acuerdo con su búsqueda. Además de hacer sugerencias para la búsqueda también busca los resultados para esta sugerencia, pero las devuelve con un menor `score`.

## Prácticas Laborales Primer Año

Con motivo de las prácticas laborales de primer año se han añadido varios subdirectorios adicionales:

* **Informe:** este directorio contiene el fichero `report.tex` que es el informe del proyecto elaborado en LaTex.
* **Presentacion:** este directorio contiene el fichero `presentation.tex` que es la presentación del proyecto elaborado en LaTex.
* **Script:** este directorio contiene el fichero `proyecto.sh` que es un script en bash que tiene las siguientes opciones:
  
  * `build`: para compilar el proyecto.
  * `run`: para compilar y ejecutar el proyecto, y abrir en el navegador la página web donde se deberán realizar las búsquedas.
  * `report`: para compilar el `.tex` y generar el `.pdf` del informe del proyecto.
  * `slides`: para compilar el `.tex` y generar el `.pdf` de la presentación del proyecto.
  * `clean`: para eliminar todos los ficheros auxiliares que no forman parte del contenido del repositorio y se generan en la compilación o ejecución del proyecto, o en la generación de los `.pdf` del reporte o la presentación
  * `veryclean`: para eliminar todos los ficheros auxiliares que no forman parte del contenido del repositorio así como los `.pdf` generados.
  * `show_report`: ejecuta un programa que permite visualizar el informe, si el fichero en `.pdf` no existe será generado. Esta opción puede recibir como parámetro el comando de la herramienta de visualización que se quiera utilizar, por defecto se abrirá con `xdg-open`.
  * `show_slides`: ejecuta un programa que permite visualizar la presentación, si el fichero en `.pdf` no existe será generado. Esta opción puede recibir como parámetro el comando de la herramienta de visualización que se quiera utilizar, por defecto se abrirá con `xdg-open`.
  * `all_docs`: para compilar el `.tex` y generar el `.pdf` tanto del informe como de la presentación del proyecto, y copiarla para el directorio principal del proyecto, además eliminará todos los ficheros auxiliares que no forman parte del contenido del repositorio.
  * `help`: muestra el menú de opciones del script.