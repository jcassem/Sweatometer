import React, { Component } from 'react';
import { Sweatometer } from './Sweatometer';

export class Find extends Component {

    constructor(props) {
        super(props);
        this.state = {
            providedWord: '',
            wordToSoundLike: '',
            hasSubmitted: false
        };

        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    handleChange(event) {
        this.setState({ providedWord: event.target.value });
    }

    handleSubmit(event) {
        this.setState({
            hasSubmitted: true,
            wordToSoundLike: this.state.providedWord
        });

        event.preventDefault();
    }

    render() {
        return (
            <div>
                <h1>Find values similar to a word</h1>
                <form onSubmit={this.handleSubmit}>
                    <label>
                        Word:
                        <input type="text" value={this.state.providedWord} onChange={this.handleChange} />
                    </label>
                    <input type="submit" value="Submit" />
                </form>
                {this.state.hasSubmitted === true ? <Sweatometer wordToSoundLike={this.state.wordToSoundLike} /> : <div />}
            </div>
        );
  }
}
