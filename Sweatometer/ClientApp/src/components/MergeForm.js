import React, { Component } from 'react';

export class MergeForm extends Component {

    constructor(props) {
        super(props);

        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    handleChange(event) {
        this.props.onChange(event.target.name, event.target.value);
    }

    handleSubmit(event) {
        this.props.onSubmit();

        event.preventDefault();
    }

    render() {
        return (
            <form onSubmit={this.handleSubmit}>
                <label>
                    Word 1:
                    <input type="text" name="firstWord" value={this.props.firstWord} onChange={this.handleChange} />
                </label>
                <label>
                    Word 2:
                    <input type="text" name="secondWord" value={this.props.secondWord} onChange={this.handleChange} />
                </label>
                <input type="submit" value="Submit" />
            </form>
        );
  }
}
