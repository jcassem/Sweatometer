import React, { Component } from 'react';

export class Sweatometer extends Component {
  static displayName = Sweatometer.name;

  constructor(props) {
    super(props);
    this.state = { forecasts: [], loading: true };
  }

  componentDidMount() {
    this.populateWeatherData();
  }

  static renderForecastsTable(similarWords) {
    return (
      <table className='table table-striped' aria-labelledby="tabelLabel">
        <thead>
          <tr>
            <th>Word</th>
            <th>Score</th>
            <th>Number of Syllables</th>
          </tr>
        </thead>
        <tbody>
          {similarWords.map(similarWord =>
            <tr key={similarWord.word}>
              <td>{similarWord.word}</td>
              <td>{similarWord.score}</td>
              <td>{similarWord.numSyllables}</td>
            </tr>
          )}
        </tbody>
      </table>
    );
  }

  render() {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : Sweatometer.renderForecastsTable(this.state.forecasts);

    return (
      <div>
        <h1 id="tabelLabel" >Sweatometer Test</h1>
        <p>Returns a list of similar sounding words to 'test'.</p>
        {contents}
      </div>
    );
  }

  async populateWeatherData() {
    const response = await fetch('wordfinder');
    const data = await response.json();
    this.setState({ forecasts: data, loading: false });
  }
}
