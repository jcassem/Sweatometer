import React, { Component } from 'react';
import { FindForm } from './FindForm';
import { SimilarWordTable } from './SimilarWordTable';

export class FindApp extends Component {

    constructor(props) {
        super(props);
        this.state = {
            searchWord: '',
            searchType: 'soundsLike',
            loading: false,
            hasRun: false,
            statusCode: 200,
            errorMessage: '',
            similarWords: []
        };

        this.handleFieldChange = this.handleFieldChange.bind(this);
        this.populateSimilarWordData = this.populateSimilarWordData.bind(this);
    }

    handleFieldChange(fieldId, value) {
        this.setState({ [fieldId]: value });
    }

    async populateSimilarWordData() {
        this.setState({
            loading: true,
            hasRun: true
        });

        try {
            const response = await fetch('api/wordfinder/find/' + this.state.searchType + "/" + this.state.searchWord);
            const status = await response.status;
            const statusText = await response.statusText;

            if (status !== 200) {
                this.setState({
                    statusCode: status,
                    errorMessage: statusText
                });
            }
            else {
                const data = await response.json();

                this.setState({
                    similarWords: data,
                    loading: false,
                    statusCode: status,
                    errorMessage: ''
                });
            }

        } catch (error) {
            this.setState({
                errorMessage: error.message
            });
        }
    }

    render() {
        let result = '';
        if (this.state.hasRun) {
            if (this.state.statusCode != 200 || this.state.errorMessage.length !== 0) {
                result =
                    <div class="alert alert-danger" role="alert">
                        <h2>{this.state.statusCode}</h2><p>{this.state.errorMessage}</p>
                    </div>;
            }
            else {
                result = <SimilarWordTable loading={this.state.loading} similarWords={this.state.similarWords} />;
            }
        }

        return (
            <div>
                <h1>Find values similar to a word</h1>
                <FindForm searchType={this.state.searchType} searchWord={this.state.searchWord}
                    onChange={this.handleFieldChange} onSubmit={this.populateSimilarWordData} />
                <br/>
                {result}
            </div>
        );
  }
}
