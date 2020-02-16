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
                <div class="form-row">
                    <div class="col-md-6 mb-6">
                        <label>Word To Add To:</label>
                        <input type="text" class="form-control" name="firstWord" value={this.props.firstWord} onChange={this.handleChange} />
                    </div>
                    <div class="col-md-6 mb-6">
                        <label>Word To Add:</label>
                        <input type="text" class="form-control" name="secondWord" value={this.props.secondWord} onChange={this.handleChange} />
                    </div>
                </div>

                <input type="submit" value="Search" class="btn btn-primary btn-lg btn-block submit" />
            </form>
        );
  }
}
