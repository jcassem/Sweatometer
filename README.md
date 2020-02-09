# Sweatometer

This project provides a series of services related to the combination of words.
The concept of 'sweat' in this context is a measure of how hard a pun/combination of words
has to work to be figured out and/or considered punny.

The main service this webapp aims to provide is the Sweatometer itself, which will take in
your word equation (word1 + word2 = yourCraftedWord), evaluate it, and return a score based on
how sweaty it is.

This service is reliant on the [Data Muse](http://www.datamuse.com/) website, which provides a series 
of word services.


## Services

Other services will include:
* Find similar words to a provided input
* Find a new combination based off multiple words provided (Coming soon)
* Equaluate a provided combination and provide a Sweat rating (Coming soon)


## Running

This is a .NET Core and React project. To get started run:
```bash
dotnet run
```