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
            similarWords: []
        };

        this.handleFieldChange = this.handleFieldChange.bind(this);
        this.mergeWords = this.mergeWords.bind(this);
    }

    handleFieldChange(fieldId, value) {
        this.setState({ [fieldId]: value });
    }

    async mergeWords() {
        this.setState({ loading: true });

        const response = await fetch('api/wordfinder/' + this.state.firstWord + "/" + this.state.secondWord);
        const data = await response.json();

        this.setState({
            similarWords: data,
            loading: false
        });
    }

    render() {
        return (
            <div>
                <h1>Find values similar to a word</h1>
                <MergeForm firstWord={this.state.firstWord} secondWord={this.state.secondWord}
                    onChange={this.handleFieldChange} onSubmit={this.mergeWords} />
                <br />
                <p>First: {this.state.firstWord} | Second: {this.state.secondWord}</p>
                <SimilarWordTable loading={this.state.loading} similarWords={this.state.similarWords} />
            </div>
        );
  }
}
