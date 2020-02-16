import React, { Component } from 'react';

export class SimilarWordTable extends Component {

    static renderSimilarWordsTable(similarWords) {
    return (
        <table className='table table-striped' aria-labelledby="tabelLabel">
            <thead>
                <tr>
                    <th>Word</th>
                    <th>Score</th>
                </tr>
            </thead>
            <tbody>
                {similarWords.map(similarWord =>
                    <tr key={similarWord.word}>
                        <td>{similarWord.word}</td>
                        <td>{similarWord.score}</td>
                    </tr>
                )}
            </tbody>
        </table>
    );
    }

    render() {
        let contents = this.props.loading
            ? <p><em>Loading...</em></p>
            : SimilarWordTable.renderSimilarWordsTable(this.props.similarWords);

    return (
        <div>
            {contents}
        </div>
    );
  }
}
