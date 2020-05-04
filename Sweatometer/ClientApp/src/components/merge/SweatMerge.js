import React, { Component } from 'react';

export class SweatMerge extends Component {

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
          </tr>
        </thead>
        <tbody>
          {similarWords.map(simWordString =>
              <tr key={simWordString}>
              <td>{simWordString}</td>
            </tr>
          )}
        </tbody>
      </table>
    );
  }

  render() {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : SweatMerge.renderSimilarWordsTable(this.state.similarWords);

    return (
        <div>
            <p>Returns options for merging '{this.props.firstWord}' and '{this.props.secondWord}'.</p>
        {contents}
      </div>
    );
  }

    async populateSimilarWordData() {
        if (this.props.wordToSoundLike != this.state.lastWordUsed) {
            const response = await fetch('api/wordfinder/' + this.props.firstWord + "/" + this.props.secondWord);
            const data = await response.json();

            this.setState({
                lastWordUsed: this.props.wordToSoundLike,
                similarWords: data,
                loading: false
            });
        }
  }
}
