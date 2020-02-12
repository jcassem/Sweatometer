import React, { Component } from 'react';
import { FindForm } from './FindForm';
import { SimilarWordTable } from './SimilarWordTable';

export class FindApp extends Component {

    constructor(props) {
        super(props);
        this.state = {
            wordToSoundLike: '',
            loading: false,
            similarWords: []
        };

        this.setWordToSoundLike = this.setWordToSoundLike.bind(this);
        this.populateSimilarWordData = this.populateSimilarWordData.bind(this);
    }

    setWordToSoundLike(updatedWordToSoundLike) {
        this.setState({ wordToSoundLike: updatedWordToSoundLike });
    }

    async populateSimilarWordData() {
        this.setState({ loading: true });

        const response = await fetch('api/wordfinder/' + this.state.wordToSoundLike);
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
                <FindForm onChange={this.setWordToSoundLike} onSubmit={this.populateSimilarWordData}/>
                <br/>
                <SimilarWordTable loading={this.state.loading} similarWords={this.state.similarWords} />
            </div>
        );
  }
}
