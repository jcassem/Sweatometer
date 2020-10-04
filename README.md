# Sweatometer

This project provides a series of services related to the combination of words.
The concept of 'sweat' in this context is a measure of how hard a pun/combination of words
has to work to be figured out and/or considered punny.

The main service this webapp aims to provide is the Sweatometer itself, which will take in
your word equation (word1 + word2 = yourCraftedWord), evaluate it, and return a score based on
how sweaty it is.

This service is reliant on the [Data Muse API](http://www.datamuse.com/api), which provides a series 
of word services.

A Docker image is in the [Github package repository](https://github.com/users/RichTeaMan/packages/container/package/sweatometer):

```bash
sudo docker run -d -p 8080:80 ghcr.io/richteaman/sweatometer
```

## Technology

This project uses .NET Core and React. A Github action pipeline runs builds and publishes Docker images.


## Services

Other services will include:
* Find similar words to a provided input
* Find a new combination based off multiple words provided
* Equaluate a provided combination and provide a Sweat rating


## Running

### Locally

```bash
cd Sweatometer
dotnet run
```

### Via Docker

```bash
cd Sweatometer
docker build -t sweatometer .
docker run -it -p 8080:80 sweatometer:latest
```

## To-dos
* Move config into advanced search options
* Add more unit tests
* Improve merge function (accuracy and efficiency)
* Make use of DataMuse filter options (could help with the above)
