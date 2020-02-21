import React, { Component } from 'react';
import { MergeForm } from './MergeForm';
import { SimilarWordTable } from './SimilarWordTable';

export class MergeApp extends Component {

    constructor(props) {
        super(props);
        this.state = {
            firstWord: '',
            secondWord: '',
            loading: false,
            hasRun: false,
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

        const response = await fetch('api/wordfinder/merge/' + this.state.firstWord + "/" + this.state.secondWord);
        const data = await response.json();

        this.setState({
            similarWords: data,
            loading: false
        });
    }

    render() {
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
                        {!this.state.hasRun ? '' : <SimilarWordTable loading={this.state.loading} similarWords={this.state.similarWords} />}
                    </div>
                </div>
            </div>
        );
  }
}
