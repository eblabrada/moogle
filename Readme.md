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

Después de hacer lo anterior necesitamos calcular la "similitud" entre el vector de `document` y el vector de la `query`, que para eso utilizo `Cosine Similarity`. Esto lo hacemos usando la fórmula:

$$cos \alpha = \frac{v_d \cdot v_q}{||v_d|| \cdot ||v_q||}$$
