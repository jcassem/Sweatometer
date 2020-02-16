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
            <div>
                <div class="alert alert-light">
                    <p>Here '<strong>Parent Word</strong>' is the word to inject '<strong>Inject Word</strong>'.</p>
                    <p>E.g. '<strong>Parent Word</strong>' = 'Cookie Monster' and <strong>Inject Word</strong>' = 'Wookie'</p>
                    <p>This will inject 'Wookie' into 'Cookie Monster' and produce 'Wookie Monster'!</p>
                </div>
                <form onSubmit={this.handleSubmit}>
                    <div class="form-row">
                        <div class="col-md-6">
                            <label>Parent Word:</label>
                            <input type="text" class="form-control" name="firstWord" value={this.props.firstWord} onChange={this.handleChange} />
                        </div>
                        <div class="col-md-6">
                            <label>Inject Word:</label>
                            <input type="text" class="form-control" name="secondWord" value={this.props.secondWord} onChange={this.handleChange} />
                        </div>
                    </div>

                    <input type="submit" value="Search" class="btn btn-primary btn-lg btn-block submit" />
                </form>

            </div>
        );
  }
}
