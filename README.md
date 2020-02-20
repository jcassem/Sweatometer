# Sweatometer

This project provides a series of services related to the combination of words.
The concept of 'sweat' in this context is a measure of how hard a pun/combination of words
has to work to be figured out and/or considered punny.

The main service this webapp aims to provide is the Sweatometer itself, which will take in
your word equation (word1 + word2 = yourCraftedWord), evaluate it, and return a score based on
how sweaty it is.

This service is reliant on the [Data Muse API](http://www.datamuse.com/api), which provides a series 
of word services.


## Technology

This project uses .NET Core and React.


## Services

Other services will include:
* Find similar words to a provided input
* Find a new combination based off multiple words provided (Coming soon)
* Equaluate a provided combination and provide a Sweat rating (Coming soon)


## Running

### Locally
```bash
dotnet run
```

### Via Docker
```bash
cd Sweatometer
docker build -t sweatometer .
docker run sweatometer:latestß
```