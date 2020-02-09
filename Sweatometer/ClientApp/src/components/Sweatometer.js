import React, { Component } from 'react';

export class Sweatometer extends Component {

  constructor(props) {
    super(props);
      this.state = {
          lastWordUsed: '',
          similarWords: [],
          loading: true
      };
  }

    componentDidMount() {
        this.populateSimilarWordData();
    }
    
    componentDidUpdate() {
        this.populateSimilarWordData();
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

    async populateSimilarWordData() {
        if (this.props.wordToSoundLike != this.state.lastWordUsed) {
            const response = await fetch('api/wordfinder/' + this.props.wordToSoundLike);
            const data = await response.json();

            this.setState({
                lastWordUsed: this.props.wordToSoundLike,
                similarWords: data,
                loading: false
            });
        }
  }
}
