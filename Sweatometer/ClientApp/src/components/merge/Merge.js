import React, { Component } from 'react';
import { SweatMerge } from './SweatMerge';

export class Merge extends Component {

    constructor(props) {
        super(props);
        this.state = {
            firstWord: '',
            secondWord: '',
            proposedFirstWord: '',
            proposedSecondWord: '',
            hasSubmitted: false
        };

        this.handleFirstWordChange = this.handleFirstWordChange.bind(this);
        this.handleSecondWordChange = this.handleSecondWordChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    handleFirstWordChange(event) {
        this.setState({
            proposedFirstWord: event.target.value
        });
    }

    handleSecondWordChange(event) {
        this.setState({
            proposedSecondWord: event.target.value
        });
    }

    handleSubmit(event) {
        this.setState({
            hasSubmitted: true,
            firstWord: this.state.proposedFirstWord,
            secondWord: this.state.proposedSecondWord
        });

        event.preventDefault();
    }

    render() {
        return (
            <div>
                <h1>Find a merge between two words:</h1>
                <form onSubmit={this.handleSubmit}>
                    <label>
                        Word:
                        <input type="text" value={this.state.proposedFirstWord} onChange={this.handleFirstWordChange} />
                    </label>
                    <label>
                        Word Two:
                        <input type="text" value={this.state.proposedSecondWord} onChange={this.handleSecondWordChange} />
                    </label>
                    <input type="submit" value="Submit" />
                </form>
                {this.state.hasSubmitted === true ? <SweatMerge firstWord={this.state.firstWord} secondWord={this.state.secondWord} /> : <div />}
            </div>
        );
  }
}
