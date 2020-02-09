import React, { Component } from 'react';

export class Sweatometer extends Component {

  constructor(props) {
    super(props);
      this.state = {
          similarWords: [],
          loading: true
      };
  }

    componentDidMount() {
    this.populateWeatherData();
  }

  static renderSimilarWordsTable(similarWords) {
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
      : Sweatometer.renderSimilarWordsTable(this.state.similarWords);

    return (
        <div>
            <p>Returns a list of similar sounding words to '{this.props.wordToSoundLike}'.</p>
        {contents}
      </div>
    );
  }

  async populateWeatherData() {
      const response = await fetch('api/wordfinder/' + this.props.wordToSoundLike);
    const data = await response.json();
    this.setState({ similarWords: data, loading: false });
  }
}
