import React, { Component } from 'react';
import { Sweatometer } from './Sweatometer';

export class Home extends Component {
    static displayName = Home.name;

    constructor(props) {
        super(props);
        this.state = {
            wordToSoundLike: '',
            hasSubmitted: false
        };

        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    handleChange(event) {
        this.setState({ wordToSoundLike: event.target.value });
    }

    handleSubmit(event) {
        var newState = this.state;
        newState.hasSubmitted = true;
        this.setState(newState);

        event.preventDefault();
    }

    render() {
        return (
            <div>
            <form onSubmit={this.handleSubmit}>
                <label>
                    Name:
                    <input type="text" value={this.state.wordToSoundLike} onChange={this.handleChange} />
                </label>
                <input type="submit" value="Submit" />
                </form>
                {this.state.hasSubmitted === true ? <Sweatometer wordToSoundLike={this.state.wordToSoundLike} /> : <div />}
            </div>
        );
  }
}
