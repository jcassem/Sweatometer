import React, { Component } from 'react';

export class SimilarWordTable extends Component {

    static renderSimilarWordsTable(similarWords) {

        if (similarWords == null || similarWords.length === 0) {
            return (<h1 class="no-merge-result">No results found</h1>);
        }
        else if (similarWords.length == 1) {
            return (
                <div class="single-merge-result">
                    <h1>{similarWords[0].word}</h1>
                    <p>Score: {similarWords[0].score}</p>
                </div>
            );
        }

        var mergeResult = similarWords[0].parentWord != null && similarWords[0].parentWord.length > 0;

        return (
            <table className='table table-striped' aria-labelledby="tabelLabel">
                <thead>
                    <tr>
                        <th>Word</th>
                        {mergeResult ? <th>Merge</th> : ''}
                        <th>Score</th>
                    </tr>
                </thead>
                <tbody>
                    {similarWords.map(similarWord =>
                        <tr key={similarWord.word}>
                            <td>{similarWord.word}</td>
                            {mergeResult ? <td>'{similarWord.parentWord}' x '{similarWord.injectedWord}'</td> : ''}
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
