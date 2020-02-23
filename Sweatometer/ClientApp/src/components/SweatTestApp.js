import React, { Component } from 'react';
import { SweatTestForm } from './SweatTestForm';
import { SweatTestResult } from './SweatTestResult';

export class SweatTestApp extends Component {

    constructor(props) {
        super(props);

        this.state = {
            firstWord: 'Elvis Presley',
            secondWord: 'Dog',
            answerWord: 'Elvis Pawlesley',
            loading: false,
            hasRun: false,
            sweatTestResult: {}
        };

        this.handleFieldChange = this.handleFieldChange.bind(this);
        this.runSweatTest = this.runSweatTest.bind(this);
    }

    handleFieldChange(fieldId, value) {
        this.setState({ [fieldId]: value });
    }

    async runSweatTest() {
        this.setState({
            loading: true,
            hasRun: true,
        });

        const response = await fetch('api/wordfinder/sweat/' + this.state.firstWord + "/" + this.state.secondWord + "/" + this.state.answerWord);
        const data = await response.json();

        this.setState({
            sweatTestResult: data,
            loading: false
        });
    }

    render() {
        return (
            <div class="container-fluid">
                <h1>Sweat Test</h1>
                <div class="row">
                    <div class="col">
                        <SweatTestForm firstWord={this.state.firstWord} secondWord={this.state.secondWord} answerWord={this.state.answerWord}
                            onChange={this.handleFieldChange} onSubmit={this.runSweatTest} />
                    </div>
                </div>
                <div class="row mt-4">
                    <div class="col">
                        {!this.state.hasRun ? '' : <SweatTestResult loading={this.state.loading} sweatTestResult={this.state.sweatTestResult} />}
                    </div>
                </div>
            </div>
        );
    }
}
