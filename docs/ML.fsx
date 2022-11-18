(**
---
title: Machine Learning
index: 21
category: Documentation
categoryindex: 0
---
*)

(*** hide ***)

(*** condition: prepare ***)
#I "../src/FSharp.Stats/bin/Release/netstandard2.0/"
#r "FSharp.Stats.dll"
#r "nuget: Plotly.NET, 2.0.0-preview.16"

(*** condition: ipynb ***)
#if IPYNB
#r "nuget: Plotly.NET, 2.0.0-preview.16"
#r "nuget: Plotly.NET.Interactive, 2.0.0-preview.16"
#r "nuget: FSharp.Stats"
#endif // IPYNB


open Plotly.NET
open Plotly.NET.StyleParam
open Plotly.NET.LayoutObjects

//some axis styling
module Chart = 
    let myAxis name = LinearAxis.init(Title=Title.init name,Mirror=StyleParam.Mirror.All,Ticks=StyleParam.TickOptions.Inside,ShowGrid=false,ShowLine=true)
    let myAxisRange name (min,max) = LinearAxis.init(Title=Title.init name,Range=Range.MinMax(min,max),Mirror=StyleParam.Mirror.All,Ticks=StyleParam.TickOptions.Inside,ShowGrid=false,ShowLine=true)
    let withAxisTitles x y chart = 
        chart 
        |> Chart.withTemplate ChartTemplates.lightMirrored
        |> Chart.withXAxis (myAxis x) 
        |> Chart.withYAxis (myAxis y)

(**

# Machine Learning

[![Binder](https://mybinder.org/badge_logo.svg)](https://mybinder.org/v2/gh/fslaborg/FSharp.Stats/gh-pages?filepath=Signal.ipynb)

_Summary:_ this tutorial demonstrates functionality relevant in the context of machine learning.

### Table of contents

 - [Dimensionality Reduction](#Dimensionality-Reduction)
    - [PCA](#PCA)

## Dimensionality Reduction

### PCA

A common approach for to reduce the dimensionality of large data sets is the use of Principal component analyis.

*)
open FSharp.Stats
open FSharp.Stats.ML.Unsupervised

let data = 
    [
        [1.0; 2.0;1.0; 2.0;];
        [1.1; 2.1;1.1; 2.1;];
        [-1.0; -2.0;1.0; 2.0;];
        [-1.1; -2.1;1.1; 2.1;];
        [-1.15; -2.15;1.15; 2.15;];
    ]
    |> FSharp.Stats.Matrix.ofJaggedList

// The PCA implementation expects column wise centered data, which can be generated by calling:
let dc = PCA.center data

// Calling compute will compute the PCA of the centered data matrix. Relevant information is stored in the result object.
let pca = PCA.compute dc

// The result of the PCA allows to visualize the analyzed data set projected onto the principal axis:

// extract components 1 and 2
let pcs = pca.PrincipalComponents |> Matrix.mapiRows (fun i v -> v.[0],v.[1])

// typical PCA "score" plot of components 1 and 2 with the explained variance indicated
let scorePlot = 
    Chart.Point(pcs)
    |> Chart.withAxisTitles (sprintf "PC1, Var explained %f" pca.VarExplainedByComponentIndividual.[0]) (sprintf "PC2, Var explained %f" pca.VarExplainedByComponentIndividual.[1])
    |> Chart.withTitle "Score Plot"

(*** condition: ipynb ***)
#if IPYNB
scorePlot
#endif // IPYNB

(***hide***)
scorePlot |> GenericChart.toChartHTML
(***include-it-raw***)

// Additionally the variable loadings can be visualized:
// Disclaimer: there is a certain ambiguity when it comes to the use of the term loading.
// To stay consistent with other implementations the term loading is used.

/// Extract loadings of the variables onto the first and second principal component
let loadings = 
    pca.Loadings 
    |> Matrix.mapRows (fun v -> v.[0],v.[1])


// typical PCA "loading" plot 
let loadingPlot = 
    loadings
    |> Seq.map (fun l -> [0.,0.;l])
    |> Seq.map Chart.Line
    |> Chart.combine
    |> Chart.withAxisTitles "PC1" "PC2"
    |> Chart.withTitle "Loading Plot"

(*** condition: ipynb ***)
#if IPYNB
loadingPlot
#endif // IPYNB

(***hide***)
loadingPlot |> GenericChart.toChartHTML
(***include-it-raw***)