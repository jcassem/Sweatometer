import React, { Component } from 'react';

export class FindForm extends Component {

    constructor(props) {
        super(props);

        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    handleChange(event) {
        this.props.onChange(event.target.value);
    }

    handleSubmit(event) {
        this.props.onSubmit();

        event.preventDefault();
    }

    render() {
        return (
            <form onSubmit={this.handleSubmit}>
                <label>
                    Word:
                    <input type="text" value={this.props.wordToSoundLike} onChange={this.handleChange} />
                </label>
                <input type="submit" value="Submit" />
            </form>
        );
  }
}
