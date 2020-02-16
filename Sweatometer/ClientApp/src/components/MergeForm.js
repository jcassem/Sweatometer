import React, { Component } from 'react';

export class MergeForm extends Component {

    static firstWordName = "Parent Word";

    static secondWordName = "Inject Word";

    constructor(props) {
        super(props);

        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    handleChange(event) {
        this.props.onChange(event.target.name, event.target.value);
    }

    handleSubmit(event) {

        let errors = "";

        if (this.props.firstWord.length <= 2) {
            errors += MergeForm.firstWordName + " must be more than two characters long.\n";
        }
        if (this.props.secondWord.length <= 2) {
            errors += MergeForm.secondWordName + " must be more than two characters long.";
        }

        if (errors.length > 0) {
            alert("Form contains errors:\n" + errors);
        }
        else {
            this.props.onSubmit();
        }

        event.preventDefault();
    }

    render() {
        return (
            <div>
                <div class="alert alert-light">
                    <p>Here '<strong>{MergeForm.firstWordName}</strong>' is the word to inject '<strong>{MergeForm.secondWordName}</strong>'.</p>
                    <p>E.g. '<strong>{MergeForm.firstWordName}</strong>' = 'Cookie Monster' and <strong>{MergeForm.secondWordName}</strong>' = 'Wookie'</p>
                    <p>This will inject 'Wookie' into 'Cookie Monster' and produce 'Wookie Monster'!</p>
                </div>
                <div>

                </div>
                <form onSubmit={this.handleSubmit} class="needs-validation" novalidate>
                    <div class="form-row">
                        <div class="col-md-6">
                            <label for="firstWord">{MergeForm.firstWordName}:</label>
                            <input type="text" class="form-control" id="firstWord" name="firstWord" value={this.props.firstWord} onChange={this.handleChange} />
                        </div>
                        <div class="col-md-6">
                            <label for="secondWord">{MergeForm.secondWordName}:</label>
                            <input type="text" class="form-control" id="secondWord" name="secondWord" value={this.props.secondWord} onChange={this.handleChange} />
                        </div>
                    </div>

                    <input type="submit" value="Search" class="btn btn-primary btn-lg btn-block submit" />
                </form>

            </div>
        );
  }
}
