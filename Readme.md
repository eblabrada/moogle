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

Lamentablemente la interfaz sigue siendo bastante pobre. Solo tuve tiempo para modificar ciertas cosas en la interfaz. 

> Nota: La verdad es que estoy esperando encontrar un "socio" en la... ehem... (ya saben 🙃) que me ayude con esta parte.

Por ahora, mientras mi búsqueda de ese "socio" no ha dado resultado; solo he arreglado algunas cosas estéticas a la hora de mostrar los resultados en la búsqueda, y en el botón de búsqueda y en la `input` de la `query`. También corregí un pequeño, pero molesto; error a la hora de escribir "Quisiste" en la sugerencia.

## Sobre el contenido a buscar

La idea original del proyecto es buscar en un conjunto de archivos de texto (con extensión `.txt`) que estén en la carpeta `Content`.

## Ejecutando el proyecto

Lo primero que tendrás que hacer para poder trabajar en este proyecto es instalar `.NET Core 6.0`. Luego, solo te debes parar en la carpeta del proyecto y ejecutar en la terminal de Linux:

```bash
make dev
```

Luego abrir tu navegador y abrir: [Moogle!](http://localhost:5000)

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

Para hacer las sugerencias de búsqueda utilizo `Levenshtein distance` calculando la mínima distancia de cada palabra de la `query` a las palabras en el vocabulario (el vocabulario contiene todas las palabras que aparecen en los documentos). Además de hacer sugerencias para la búsqueda también busca los resultados para esta sugerencia, pero las devuelve con un menor `score`.  