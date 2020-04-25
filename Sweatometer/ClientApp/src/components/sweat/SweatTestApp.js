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
            statusCode: 200,
            errorMessage: '',
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

        try {
            const response = await fetch('api/wordfinder/sweat/' + this.state.firstWord + "/" + this.state.secondWord + "/" + this.state.answerWord);
            const status = await response.status;
            const statusText = await response.statusText;

            if (status !== 200) {
                this.setState({
                    statusCode: status,
                    errorMessage: statusText
                });
            }
            else {
                const data = await response.json();

                this.setState({
                    sweatTestResult: data,
                    loading: false,
                    statusCode: status,
                    errorMessage: ''
                });
            }

        } catch(error) {
            this.setState({
                errorMessage: error.message
            });
        }
    }

    render() {
        let result = '';
        if (this.state.hasRun) {
            if (this.state.statusCode != 200 || this.state.errorMessage.length !== 0) {
                result =
                    <div class="alert alert-danger" role="alert">
                        <h2>{this.state.statusCode}</h2><p>{this.state.errorMessage}</p>
                    </div>;
            }
            else {
                result = <SweatTestResult loading={this.state.loading} sweatTestResult={this.state.sweatTestResult} />;
            }
        }

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
                        {result}
                    </div>
                </div>
            </div>
        );
    }
}
