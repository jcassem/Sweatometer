import React, { Component } from 'react';
import { FindForm } from './FindForm';
import { SimilarWordTable } from './SimilarWordTable';

export class FindApp extends Component {

    constructor(props) {
        super(props);
        this.state = {
            wordToSoundLike: '',
            searchType: 'soundsLike',
            loading: false,
            similarWords: []
        };

        this.handleFieldChange = this.handleFieldChange.bind(this);
        this.populateSimilarWordData = this.populateSimilarWordData.bind(this);
    }

    handleFieldChange(fieldId, value) {
        this.setState({ [fieldId]: value });
    }

    async populateSimilarWordData() {
        this.setState({ loading: true });

        const response = await fetch('api/wordfinder/find/' + this.state.searchType + "/" + this.state.wordToSoundLike);
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
                <FindForm searchType={this.state.searchType}
                    onChange={this.handleFieldChange} onSubmit={this.populateSimilarWordData} />
                <br/>
                <SimilarWordTable loading={this.state.loading} similarWords={this.state.similarWords} />
            </div>
        );
  }
}
