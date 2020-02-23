import React, { Component } from 'react';
import { SimilarWordTable } from './SimilarWordTable';

export class SweatTestResult extends Component {

    static renderSweatTestResult(testResult) {

        if (testResult === null || testResult.result == 'NOT_FOUND') {
            return <p><em>No result found</em></p>;
        }

        let alternatives = testResult.alternatives !== null && testResult.alternatives.length > 0 ?
            <div class="row mt-4">
                <div class="col">
                    <h2>Alternatives:</h2>
                    <SimilarWordTable similarWords={testResult.alternatives} />
                </div>
            </div>
            : '';

        return (
            <div>
                <div>
                    <h2>Outcome: {testResult.outcome}</h2>
                    <h3>Score: {testResult.score}</h3>
                </div>
                {alternatives}
            </div>
        )
    }

    render() {
        let contents = this.props.loading
            ? <p><em>Loading...</em></p>
            : SweatTestResult.renderSweatTestResult(this.props.sweatTestResult);

    return (
        <div>
            {contents}
        </div>
    );
  }
}
