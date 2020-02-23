import React, { Component } from 'react';
import { MergeForm } from './MergeForm';
import { SimilarWordTable } from './SimilarWordTable';

export class MergeApp extends Component {

    constructor(props) {
        super(props);
        this.state = {
            firstWord: 'Jimmy Hendrix',
            secondWord: 'Dog',
            loading: false,
            hasRun: false,
            statusCode: 200,
            errorMessage: '',
            similarWords: []
        };

        this.handleFieldChange = this.handleFieldChange.bind(this);
        this.mergeWords = this.mergeWords.bind(this);
    }

    handleFieldChange(fieldId, value) {
        this.setState({ [fieldId]: value });
    }

    async mergeWords() {
        this.setState({
            loading: true,
            hasRun: true,
        });


        try {
            let response = await fetch('api/wordfinder/merge/' + this.state.firstWord + "/" + this.state.secondWord)
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
                    similarWords: data,
                    loading: false,
                    statusCode: response.status,
                    errorMessage: ''
                });
            }
            
        } catch (error) {
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
                result = <SimilarWordTable loading={this.state.loading} similarWords={this.state.similarWords} />;
            }
        }

        return (
            <div class="container-fluid">
                <h1>Merge two words</h1>
                <div class="row">
                    <div class="col">
                        <MergeForm firstWord={this.state.firstWord} secondWord={this.state.secondWord}
                            onChange={this.handleFieldChange} onSubmit={this.mergeWords} />
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
